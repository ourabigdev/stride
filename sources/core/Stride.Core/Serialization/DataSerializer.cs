// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#pragma warning disable SA1402 // File may only contain a single class
#pragma warning disable SA1025 // Code must not contain multiple whitespace in a row

using System.Runtime.CompilerServices;
using Stride.Core.Storage;

namespace Stride.Core.Serialization;

/// <summary>
/// Describes how to serialize and deserialize an object without knowing its type.
/// Used as a common base class for all data serializers.
/// </summary>
public abstract partial class DataSerializer
{
    /// <summary>
    /// The type id of <see cref="SerializationType"/>. Used internally to avoid dealing with strings.
    /// </summary>
    public ObjectId SerializationTypeId;

    /// <summary>
    /// Used internally to know if the serializer has been initialized.
    /// </summary>
    internal bool Initialized;
    internal SpinLock InitializeLock = new(true);

    /// <summary>
    /// The type of the object that can be serialized or deserialized.
    /// </summary>
    public abstract Type SerializationType { get; }

    public abstract bool IsBlittable { get; }

    /// <summary>
    /// Initializes the specified serializer.
    /// </summary>
    /// <remarks>This method should be thread-safe and OK to call multiple times.</remarks>
    /// <param name="serializerSelector">The serializer.</param>
    public virtual void Initialize(SerializerSelector serializerSelector)
    {
    }

    /// <summary>
    /// Serializes or deserializes the given object <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to serialize or deserialize.</param>
    /// <param name="mode">The serialization mode.</param>
    /// <param name="stream">The stream to serialize or deserialize to.</param>
    public abstract void Serialize(ref object obj, ArchiveMode mode, SerializationStream stream);

    /// <summary>
    /// Performs the first step of serialization or deserialization.
    /// </summary>
    /// <remarks>
    /// Typically, it will instantiate the object if [null], and if it's a collection clear it.
    /// </remarks>
    /// <param name="obj">The object to process.</param>
    /// <param name="mode">The serialization mode.</param>
    /// <param name="stream">The stream to serialize or deserialize to.</param>
    public abstract void PreSerialize(ref object obj, ArchiveMode mode, SerializationStream stream);
}

/// <summary>
/// Describes how to serialize and deserialize an object of a given type.
/// </summary>
/// <typeparam name="T">The type of object to serialize or deserialize.</typeparam>
public abstract class DataSerializer<T> : DataSerializer
{
    /// <inheritdoc/>
    public override Type SerializationType => typeof(T);

    /// <inheritdoc/>
    public override bool IsBlittable => false;

    /// <inheritdoc/>
    public override void Serialize(ref object obj, ArchiveMode mode, SerializationStream stream)
    {
        var objT = obj == null ? default : (T)obj;
        Serialize(ref objT, mode, stream);
        obj = objT;
    }

    /// <summary>
    /// Serializes the given object <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to serialize or deserialize.</param>
    /// <param name="stream">The stream to serialize or deserialize to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Serialize(T obj, SerializationStream stream)
    {
        Serialize(ref obj, ArchiveMode.Serialize, stream);
    }

    /// <inheritdoc/>
    public override void PreSerialize(ref object obj, ArchiveMode mode, SerializationStream stream)
    {
        var objT = obj == null ? default : (T)obj;
        PreSerialize(ref objT, mode, stream);
        obj = objT;
    }

    /// <summary>
    /// Performs the first step of serialization or deserialization.
    /// </summary>
    /// <remarks>
    /// Typically, it will instantiate the object if [null], and if it's a collection clear it.
    /// </remarks>
    /// <param name="obj">The object to process.</param>
    /// <param name="mode">The serialization mode.</param>
    /// <param name="stream">The stream to serialize or deserialize to.</param>
    public virtual void PreSerialize(ref T obj, ArchiveMode mode, SerializationStream stream)
    {
    }

    /// <summary>
    /// Serializes or deserializes the given object <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to serialize or deserialize.</param>
    /// <param name="mode">The serialization mode.</param>
    /// <param name="stream">The stream to serialize or deserialize to.</param>
    public abstract void Serialize(ref T obj, ArchiveMode mode, SerializationStream stream);
}
