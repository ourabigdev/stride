// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Stride.Core;

/// <summary>
/// A struct to dispose <see cref="IDisposable"/>, <see cref="IReferencable"/> instances and allocated unmanaged memory.
/// </summary>
public struct ObjectCollector : IDisposable
{
    private List<object> disposables;

    /// <summary>
    /// Gets the number of elements to dispose.
    /// </summary>
    /// <value>The number of elements to dispose.</value>
    public readonly int Count => disposables.Count;

    /// <summary>
    /// Disposes all object collected by this class and clear the list. The collector can still be used for collecting.
    /// </summary>
    /// <remarks>
    /// To completely dispose this instance and avoid further dispose, use <see cref="Dispose"/> method instead.
    /// </remarks>
    public readonly void Dispose()
    {
        if (disposables == null)
        {
            return;
        }

        for (var i = disposables.Count - 1; i >= 0; i--)
        {
            var objectToDispose = disposables[i];
            DisposeObject(objectToDispose);
            disposables.RemoveAt(i);
        }
        disposables.Clear();
    }

    public void EnsureValid()
    {
        disposables ??= [];
    }

    /// <summary>
    /// Adds a <see cref="IDisposable"/> object or a <see cref="IntPtr"/> allocated using <see cref="Utilities.AllocateMemory"/> to the list of the objects to dispose.
    /// </summary>
    /// <param name="objectToDispose">To dispose.</param>
    /// <exception cref="ArgumentException">If objectToDispose argument is not IDisposable or a valid memory pointer allocated by <see cref="Utilities.AllocateMemory"/></exception>
    public T Add<T>(T objectToDispose)
        where T : notnull
    {
        if (!(objectToDispose is IDisposable || objectToDispose is IntPtr || objectToDispose is IReferencable))
            throw new ArgumentException("Argument must be IDisposable, IReferenceable or IntPtr");

        // Check memory alignment
        if (objectToDispose is IntPtr memoryPtr && !Utilities.IsMemoryAligned(memoryPtr))
        {
            throw new ArgumentException("Memory pointer is invalid. Memory must have been allocated with Utilties.AllocateMemory");
        }

        EnsureValid();

        if (!disposables.Contains(objectToDispose))
            disposables.Add(objectToDispose);

        return objectToDispose;
    }

    /// <summary>
    /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
    /// </summary>
    /// <param name="objectToDispose">Object to dispose.</param>
    public readonly void RemoveAndDispose<T>([MaybeNull] ref T objectToDispose)
        where T : notnull
    {
        if (disposables != null)
        {
            Remove(objectToDispose);
            DisposeObject(objectToDispose);
            objectToDispose = default;
        }
    }

    /// <summary>
    /// Removes a disposable object to the list of the objects to dispose.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectToDispose">To dispose.</param>
    public readonly void Remove<T>(T objectToDispose)
        where T : notnull
    {
        disposables?.Remove(objectToDispose);
    }

    private static void DisposeObject(object objectToDispose)
    {
        if (objectToDispose is IReferencable referenceableObject)
        {
            referenceableObject.Release();
            return;
        }

        if (objectToDispose is IDisposable disposableObject)
        {
            disposableObject.Dispose();
        }
        else
        {
            var localData = objectToDispose;
            var dataPointer = (IntPtr)localData;
            Utilities.FreeMemory(dataPointer);
        }
    }
}
