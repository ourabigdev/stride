// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Reflection;
using Stride.Core.Reflection;

namespace Stride.Core.Yaml;

/// <summary>
/// Base class for Yaml serializer
/// </summary>
public abstract class YamlSerializerBase : IDisposable
{
    // TODO: This code is not robust in case of reloading assemblies into the same process
    protected readonly List<Assembly> RegisteredAssemblies = [];
    protected readonly object Lock = new();

    protected YamlSerializerBase()
    {
        AssemblyRegistry.AssemblyRegistered += AssemblyRegistered;
        AssemblyRegistry.AssemblyUnregistered += AssemblyUnregistered;
        RegisteredAssemblies.AddRange(AssemblyRegistry.FindAll());
    }

    public void Dispose()
    {
        AssemblyRegistry.AssemblyRegistered -= AssemblyRegistered;
        AssemblyRegistry.AssemblyUnregistered -= AssemblyUnregistered;
        lock (Lock)
        {
            RegisteredAssemblies.Clear();
        }
    }

    /// <summary>
    /// Reset the assembly cache used by this class.
    /// </summary>
    public virtual void ResetCache()
    {
    }

    private void AssemblyRegistered(object? sender, AssemblyRegisteredEventArgs e)
    {
        // Process only our own assemblies
        if (!e.Categories.Contains(AssemblyCommonCategories.Engine))
            return;

        lock (Lock)
        {
            RegisteredAssemblies.Add(e.Assembly);

            // Reset the current serializer as the set of assemblies has changed
            ResetCache();
        }
    }

    private void AssemblyUnregistered(object? sender, AssemblyRegisteredEventArgs e)
    {
        // Process only our own assemblies
        if (!e.Categories.Contains(AssemblyCommonCategories.Engine))
            return;

        lock (Lock)
        {
            RegisteredAssemblies.Remove(e.Assembly);

            // Reset the current serializer as the set of assemblies has changed
            ResetCache();
        }
    }
}
