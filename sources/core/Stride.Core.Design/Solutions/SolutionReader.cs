#region License

// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// This file is distributed under MIT License. See LICENSE.md for details.
//
// SLNTools
// Copyright (c) 2009
// by Christian Warren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#endregion

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Stride.Core.Solutions;

internal class SolutionReader : IDisposable
{
    private static readonly Regex RegexConvertEscapedValues = new(@"\\u(?<HEXACODE>[0-9a-fA-F]{4})");
    private static readonly Regex RegexParseGlobalSection = new(@"^(?<TYPE>GlobalSection)\((?<NAME>.*)\) = (?<STEP>.*)$");
    private static readonly Regex RegexParseProject = new("^Project\\(\"(?<PROJECTTYPEGUID>.*)\"\\)\\s*=\\s*\"(?<PROJECTNAME>.*)\"\\s*,\\s*\"(?<RELATIVEPATH>.*)\"\\s*,\\s*\"(?<PROJECTGUID>.*)\"$");
    private static readonly Regex RegexParseProjectConfigurationPlatformsName = new(@"^(?<GUID>\{[-0-9a-zA-Z]+\})\.(?<DESCRIPTION>.*)$");
    private static readonly Regex RegexParseProjectSection = new(@"^(?<TYPE>ProjectSection)\((?<NAME>.*)\) = (?<STEP>.*)$");
    private static readonly Regex RegexParsePropertyLine = new(@"^(?<PROPERTYNAME>[^=]*)\s*=\s*(?<PROPERTYVALUE>[^=]*)$");
    private static readonly Regex RegexParseVersionControlName = new("^(?<NAME_WITHOUT_INDEX>[a-zA-Z]*)(?<INDEX>[0-9]+)$");
    private readonly string solutionFullPath;
    private Solution solution;
    private int currentLineNumber;
    private StreamReader? reader;
    private bool disposed;

    public SolutionReader(string solutionFullPath) : this(solutionFullPath, new FileStream(solutionFullPath, FileMode.Open, FileAccess.Read))
    {
    }

    public SolutionReader(string solutionFullPath, Stream reader)
    {
        this.solutionFullPath = solutionFullPath;
        this.reader = new StreamReader(reader, Encoding.Default);
        currentLineNumber = 0;
    }

    public void Dispose()
    {
        disposed = true;
        if (reader is not null)
        {
            reader.Dispose();
            reader = null!;
        }
    }

    public Solution ReadSolutionFile()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(disposed, this);
#else
        if (disposed) throw new ObjectDisposedException(nameof(SolutionReader));
#endif
        lock (reader!)
        {
            solution = new Solution();
            ReadHeader();
            for (var line = ReadLine(); line != null; line = ReadLine())
            {
                // Skip blank lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("Project(", StringComparison.InvariantCultureIgnoreCase))
                {
                    solution.Projects.Add(ReadProject(line));
                }
                else if (string.Equals(line, "Global", StringComparison.InvariantCultureIgnoreCase))
                {
                    ReadGlobal();
                    // TODO valide end of file
                    break;
                }
                else if (RegexParsePropertyLine.Match(line).Success)
                {
                    // Read VS properties (introduced in VS2012/VS2013?)
                    solution.Properties.Add(ReadPropertyLine(line));
                }
                else
                {
                    throw new SolutionFileException($"Invalid line read on line #{currentLineNumber}.\nFound: {line}\nExpected: A line beginning with 'Project(' or 'Global'.");
                }
            }
            return solution;
        }
    }

    private Project FindProjectByGuid(string guid, int lineNumber)
    {
        return solution.Projects.FindByGuid(new Guid(guid))
            ?? throw new SolutionFileException($"Invalid guid found on line #{lineNumber}.\nFound: {guid}\nExpected: A guid from one of the projects in the solution.");
    }

    private void HandleNestedProjects(string name, string type, string step, List<PropertyItem> propertyLines, int startLineNumber)
    {
        var localLineNumber = startLineNumber;
        foreach (var propertyLine in propertyLines)
        {
            localLineNumber++;
            var left = FindProjectByGuid(propertyLine.Name, localLineNumber);
            left.ParentGuid = new Guid(propertyLine.Value);
        }
        solution.GlobalSections.Add(
            new Section(
                name,
                type,
                step,
                null));
    }

    private void HandleProjectConfigurationPlatforms(string name, string type, string step, List<PropertyItem> propertyLines, int startLineNumber)
    {
        var localLineNumber = startLineNumber;
        foreach (var propertyLine in propertyLines)
        {
            localLineNumber++;
            var match = RegexParseProjectConfigurationPlatformsName.Match(propertyLine.Name);
            if (!match.Success)
            {
                throw new SolutionFileException($"Invalid format for a project configuration name on line #{currentLineNumber}.\nFound: {propertyLine.Name}");
            }

            var projectGuid = match.Groups["GUID"].Value;
            var description = match.Groups["DESCRIPTION"].Value;
            var left = FindProjectByGuid(projectGuid, localLineNumber);
            left.PlatformProperties.Add(
                new PropertyItem(
                    description,
                    propertyLine.Value));
        }
        solution.GlobalSections.Add(
            new Section(
                name,
                type,
                step,
                null));
    }

    private void HandleVersionControlLines(string name, string type, string step, List<PropertyItem> propertyLines)
    {
        var propertyLinesByIndex = new Dictionary<int, List<PropertyItem>>();
        var othersVersionControlLines = new List<PropertyItem>();
        foreach (var propertyLine in propertyLines)
        {
            var match = RegexParseVersionControlName.Match(propertyLine.Name);
            if (match.Success)
            {
                var nameWithoutIndex = match.Groups["NAME_WITHOUT_INDEX"].Value.Trim();
                var index = int.Parse(match.Groups["INDEX"].Value.Trim());

                if (!propertyLinesByIndex.TryGetValue(index, out var value))
                {
                    value = [];
                    propertyLinesByIndex[index] = value;
                }

                value.Add(new PropertyItem(nameWithoutIndex, propertyLine.Value));
            }
            else
            {
                // Ignore SccNumberOfProjects. This number will be computed and added by the SolutionFileWriter class.
                if (propertyLine.Name != "SccNumberOfProjects")
                {
                    othersVersionControlLines.Add(propertyLine);
                }
            }
        }

        // Handle the special case for the solution itself.
        othersVersionControlLines.Add(new PropertyItem("SccLocalPath0", "."));

        foreach (var item in propertyLinesByIndex)
        {
            var index = item.Key;
            var propertiesForIndex = item.Value;

            var uniqueNameProperty = propertiesForIndex.Find(property => property.Name == "SccProjectUniqueName");
            // If there is no ProjectUniqueName, we assume that it's the entry related to the solution by itself. We
            // can ignore it because we added the special case above.
            if (uniqueNameProperty != null)
            {
                var uniqueName = RegexConvertEscapedValues.Replace(uniqueNameProperty.Value, match =>
                {
                    var hexaValue = int.Parse(match.Groups["HEXACODE"].Value, NumberStyles.AllowHexSpecifier);
                    return char.ConvertFromUtf32(hexaValue);
                });
                uniqueName = uniqueName.Replace(@"\\", @"\");

                Project? relatedProject = null;
                foreach (var project in solution.Projects)
                {
                    if (string.Equals(project.GetRelativePath(solution), uniqueName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        relatedProject = project;
                    }
                }
                if (relatedProject == null)
                {
                    throw new SolutionFileException(
                        $"Invalid value for the property 'SccProjectUniqueName{index}' of the global section '{name}'.\nFound: {uniqueName}\nExpected: A value equal to the field 'RelativePath' of one of the projects in the solution.");
                }

                relatedProject.VersionControlProperties.AddRange(propertiesForIndex);
            }
        }

        solution.GlobalSections.Add(
            new Section(
                name,
                type,
                step,
                othersVersionControlLines));
    }

    private void ReadGlobal()
    {
        for (var line = ReadLine(); !line.StartsWith("EndGlobal", StringComparison.Ordinal); line = ReadLine())
        {
            ReadGlobalSection(line);
        }
    }

    private void ReadGlobalSection(string firstLine)
    {
        var match = RegexParseGlobalSection.Match(firstLine);
        if (!match.Success)
        {
            throw new SolutionFileException($"Invalid format for a global section on line #{currentLineNumber}.\nFound: {firstLine}");
        }

        var type = match.Groups["TYPE"].Value.Trim();
        var name = match.Groups["NAME"].Value.Trim();
        var step = match.Groups["STEP"].Value.Trim();

        var propertyLines = new List<PropertyItem>();
        var startLineNumber = currentLineNumber;
        var endOfSectionToken = "End" + type;
        for (var line = ReadLine(); !line.StartsWith(endOfSectionToken, StringComparison.InvariantCultureIgnoreCase); line = ReadLine())
        {
            propertyLines.Add(ReadPropertyLine(line));
        }

        switch (name)
        {
            case "NestedProjects":
                HandleNestedProjects(name, type, step, propertyLines, startLineNumber);
                break;

            case "ProjectConfigurationPlatforms":
                HandleProjectConfigurationPlatforms(name, type, step, propertyLines, startLineNumber);
                break;

            default:
                if (name.EndsWith("Control", StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleVersionControlLines(name, type, step, propertyLines);
                }
                else
                {
                    solution.GlobalSections.Add(
                        new Section(
                            name,
                            type,
                            step,
                            propertyLines));
                }
                break;
        }
    }

    private void ReadHeader()
    {
        for (var i = 1; i <= 3; i++)
        {
            var line = ReadLine();
            solution.Headers.Add(line);
#if NETCOREAPP2_0_OR_GREATER
            if (line.StartsWith('#'))
#else
            if (line.StartsWith("#"))
#endif
            {
                return;
            }
        }
    }

    private string ReadLine()
    {
        var line = reader!.ReadLine()
            ?? throw new SolutionFileException("Unexpected end of file encounted while reading the solution file.");
        currentLineNumber++;
        return line.Trim();
    }

    private Project ReadProject(string firstLine)
    {
        var match = RegexParseProject.Match(firstLine);
        if (!match.Success)
        {
            throw new SolutionFileException($"Invalid format for a project on line #{currentLineNumber}.\nFound: {firstLine}.");
        }

        var projectTypeGuid = new Guid(match.Groups["PROJECTTYPEGUID"].Value.Trim());
        var projectName = match.Groups["PROJECTNAME"].Value.Trim();
        var relativePath = match.Groups["RELATIVEPATH"].Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
        var projectGuid = new Guid(match.Groups["PROJECTGUID"].Value.Trim());

        var projectSections = new List<Section>();
        for (var line = ReadLine(); !line.StartsWith("EndProject", StringComparison.Ordinal); line = ReadLine())
        {
            projectSections.Add(ReadProjectSection(line));
        }

        return new Project(
            projectGuid,
            projectTypeGuid,
            projectName,
            projectTypeGuid == KnownProjectTypeGuid.SolutionFolder ? relativePath : Path.Combine(Path.GetDirectoryName(solutionFullPath), relativePath),
            Guid.Empty,
            projectSections,
            null,
            null);
    }

    private Section ReadProjectSection(string firstLine)
    {
        var match = RegexParseProjectSection.Match(firstLine);
        if (!match.Success)
        {
            throw new SolutionFileException($"Invalid format for a project section on line #{currentLineNumber}.\nFound: {firstLine}.");
        }

        var type = match.Groups["TYPE"].Value.Trim();
        var name = match.Groups["NAME"].Value.Trim();
        var step = match.Groups["STEP"].Value.Trim();

        var propertyLines = new List<PropertyItem>();
        var endOfSectionToken = "End" + type;
        for (var line = ReadLine(); !line.StartsWith(endOfSectionToken, StringComparison.InvariantCultureIgnoreCase); line = ReadLine())
        {
            propertyLines.Add(ReadPropertyLine(line));
        }
        return new Section(name, type, step, propertyLines);
    }

    private PropertyItem ReadPropertyLine(string line)
    {
        var match = RegexParsePropertyLine.Match(line);
        if (!match.Success)
        {
            throw new SolutionFileException($"Invalid format for a property on line #{currentLineNumber}.\nFound: {line}.");
        }

        return new PropertyItem(
            match.Groups["PROPERTYNAME"].Value.Trim(),
            match.Groups["PROPERTYVALUE"].Value.Trim());
    }
}
