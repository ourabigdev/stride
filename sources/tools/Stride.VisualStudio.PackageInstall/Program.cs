// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Diagnostics;
using Stride.Core.CodeEditorSupport.VisualStudio;

namespace Stride.VisualStudio.PackageInstall;

static class Program
{
    static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                throw new Exception("Expecting a parameter such as /install, /repair or /uninstall");
            }

            const string vsixFile = "Stride.vsix";

            // Locate a VS installation with VSIXInstaller.exe.
            // Select the latest version of VS possible, in case there is some bugfixes or incompatible changes.
            var visualStudioVersionByVsixVersion = VisualStudioVersions.AvailableInstances.Where(x => x.HasVsixInstaller);
            var ideInfo = visualStudioVersionByVsixVersion.OrderByDescending(x => x.InstallationVersion).FirstOrDefault(x => File.Exists(x.VsixInstallerPath));
            if (ideInfo == null)
            {
                throw new InvalidOperationException($"Could not find a proper installation of Visual Studio 2019 or later");
            }

            switch (args[0])
            {
                case "/install":
                case "/repair":
                {
                    // Install VSIX
                    var exitCode = RunVsixInstaller(ideInfo.VsixInstallerPath, "\"" + vsixFile + "\"");
                    if (exitCode != 0)
                        throw new InvalidOperationException($"VSIX Installer didn't run properly: exit code {exitCode}");
                    break;
                }

                case "/uninstall":
                {
                    // Note: we allow uninstall to fail (i.e. VSIX was not installed for that specific Visual Studio version)
                    RunVsixInstaller(ideInfo.VsixInstallerPath, "/uninstall:Stride.VisualStudio.Package.2022 /quiet");
                    break;
                }
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            return 1;
        }
    }

    /// <summary>
    /// Starts the VSIX installer at the given path with the given argument, and waits for the process to exit before returning.
    /// </summary>
    /// <param name="pathToVsixInstaller">The path to a VSIX installer provided by a version of Visual Studio.</param>
    /// <param name="arguments">The arguments to pass to the VSIX installer.</param>
    /// <returns><c>True</c> if the VSIX installer exited with code 0, <c>False</c> otherwise.</returns>
    private static int RunVsixInstaller(string pathToVsixInstaller, string arguments)
    {
        var process = Process.Start(pathToVsixInstaller, arguments);
        if (process == null)
        {
            return -1;
        }
        process.WaitForExit();
        return process.ExitCode;
    }
}
