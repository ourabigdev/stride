// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Reflection;

namespace Stride.Core.Reflection;

/// <summary>
/// A visitor for serializable data (binary, yaml and editor).
/// </summary>
public abstract class DataVisitorBase : IDataVisitor
{
    private readonly HashSet<object> visitedObjects = new(ReferenceEqualityComparer<object>.Default);
    private readonly Dictionary<Type, IDataCustomVisitor> mapTypeToCustomVisitors = [];
    private VisitorContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataVisitorBase"/> class.
    /// </summary>
    /// <param name="attributeRegistry">The attribute registry.</param>
    protected DataVisitorBase(IAttributeRegistry attributeRegistry)
        : this(new TypeDescriptorFactory(attributeRegistry))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataVisitorBase"/> class.
    /// </summary>
    protected DataVisitorBase()
        : this(Reflection.TypeDescriptorFactory.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataVisitorBase"/> class.
    /// </summary>
    /// <param name="typeDescriptorFactory">The type descriptor factory.</param>
    /// <exception cref="ArgumentNullException">typeDescriptorFactory</exception>
    protected DataVisitorBase(ITypeDescriptorFactory typeDescriptorFactory)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(typeDescriptorFactory);
#else
        if (typeDescriptorFactory is null) throw new ArgumentNullException(nameof(typeDescriptorFactory));
#endif
        TypeDescriptorFactory = typeDescriptorFactory;
        CustomVisitors = [];
        context.DescriptorFactory = TypeDescriptorFactory;
        context.Visitor = this;
        CurrentPath = new MemberPath(16);
    }

    /// <summary>
    /// Gets the type descriptor factory.
    /// </summary>
    /// <value>The type descriptor factory.</value>
    public ITypeDescriptorFactory TypeDescriptorFactory { get; }

    /// <summary>
    /// Gets or sets the custom visitors.
    /// </summary>
    /// <value>The custom visitors.</value>
    public List<IDataCustomVisitor> CustomVisitors { get; }

    /// <summary>
    /// Gets the current member path being visited.
    /// </summary>
    /// <value>The current path.</value>
    protected MemberPath CurrentPath { get; }

    /// <summary>
    /// Gets the attribute registry.
    /// </summary>
    /// <value>The attribute registry.</value>
    public IAttributeRegistry AttributeRegistry => TypeDescriptorFactory.AttributeRegistry;

    /// <summary>
    /// Resets this instance (clears the cache of visited objects).
    /// </summary>
    public virtual void Reset()
    {
        visitedObjects.Clear();
    }

    /// <summary>
    /// Visits the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    public void Visit(object? obj)
    {
        Visit(obj, null);
    }

    /// <summary>
    /// Visits the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="descriptor">The descriptor.</param>
    /// <exception cref="ArgumentNullException">
    /// obj
    /// or
    /// descriptor
    /// </exception>
    /// <exception cref="ArgumentException">Descriptor [{0}] type doesn't correspond to object type [{1}].ToFormat(descriptor.Type, obj.GetType())</exception>
    protected void Visit(object? obj, ITypeDescriptor? descriptor)
    {
        if (obj == null)
        {
            VisitNull();
            return;
        }

        var objectType = obj.GetType();
        if (descriptor == null || descriptor.Type != objectType)
        {
            descriptor = TypeDescriptorFactory.Find(objectType);
        }

        if (descriptor is NullableDescriptor descriptor1)
        {
            descriptor = TypeDescriptorFactory.Find(descriptor1.UnderlyingType);
        }

        context.Instance = obj;
        context.Descriptor = (ObjectDescriptor)descriptor!;

        switch (descriptor!.Category)
        {
            case DescriptorCategory.Primitive:
                VisitPrimitive(obj, (PrimitiveDescriptor)descriptor);
                break;
            default:
                // Note that the behaviour is slightly different if a type has a custom visitor or not.
                // If it has a custom visitor, it will visit the object even if the object has been already visited
                // otherwise it will use CanVisit() on this instance. The CanVisit() is tracking a list of
                // visited objects and it will not revisit the object.
                IDataCustomVisitor? customVisitor;
                if (!mapTypeToCustomVisitors.TryGetValue(objectType, out customVisitor) && CustomVisitors.Count > 0)
                {
                    for (var i = CustomVisitors.Count - 1; i >= 0; i--)
                    {
                        var dataCustomVisitor = CustomVisitors[i];
                        if (dataCustomVisitor.CanVisit(objectType))
                        {
                            customVisitor = dataCustomVisitor;
                            mapTypeToCustomVisitors.Add(objectType, dataCustomVisitor);
                            break;
                        }
                    }
                }

                if (customVisitor != null)
                {
                    customVisitor.Visit(ref context);
                }
                else if (CanVisit(obj))
                {
                    VisitObject(obj, context.Descriptor, true);
                }
                break;
        }
    }

    /// <inheritdoc />
    public virtual void VisitNull()
    {
    }

    /// <inheritdoc />
    public virtual void VisitPrimitive(object primitive, PrimitiveDescriptor descriptor)
    {
    }

    /// <inheritdoc />
    public virtual void VisitObject(object obj, ObjectDescriptor descriptor, bool visitMembers)
    {
        if (!obj.GetType().IsArray && visitMembers)
        {
            foreach (var member in descriptor.Members)
            {
                CurrentPath.Push(member);
                VisitObjectMember(obj, descriptor, member, member.Get(obj));
                CurrentPath.Pop();
            }
        }

        switch (descriptor.Category)
        {
            case DescriptorCategory.Array:
                VisitArray((Array)obj, (ArrayDescriptor)descriptor);
                break;
            case DescriptorCategory.List:
            case DescriptorCategory.Collection:
                VisitCollection((IEnumerable)obj, (CollectionDescriptor)descriptor);
                break;
            case DescriptorCategory.Dictionary:
                VisitDictionary(obj, (DictionaryDescriptor)descriptor);
                break;
            case DescriptorCategory.Set:
                VisitSet((IEnumerable)obj, (SetDescriptor)descriptor);
                break;
        }
    }

    /// <inheritdoc />
    public virtual void VisitObjectMember(object container, ObjectDescriptor containerDescriptor, IMemberDescriptor member, object? value)
    {
        Visit(value, member.TypeDescriptor);
    }

    /// <inheritdoc />
    public virtual void VisitArray(Array array, ArrayDescriptor descriptor)
    {
        for (var i = 0; i < array.Length; i++)
        {
            var value = array.GetValue(i);
            CurrentPath.Push(descriptor, i);
            VisitArrayItem(array, descriptor, i, value, TypeDescriptorFactory.Find(value?.GetType() ?? descriptor.ElementType));
            CurrentPath.Pop();
        }
    }

    /// <inheritdoc />
    public virtual void VisitArrayItem(Array array, ArrayDescriptor descriptor, int index, object? item, ITypeDescriptor? itemDescriptor)
    {
        Visit(item, itemDescriptor);
    }

    /// <inheritdoc />
    public virtual void VisitCollection(IEnumerable collection, CollectionDescriptor descriptor)
    {
        if (descriptor.Category == DescriptorCategory.Set)
        {
            throw new ArgumentException("Shouldn't call VisitCollection() to visit a set");
        }
        var i = 0;

        // Make a copy in case VisitCollectionItem mutates something
        foreach (var item in collection.Cast<object>().ToList())
        {
            CurrentPath.Push(descriptor, i);
            VisitCollectionItem(collection, descriptor, i, item, TypeDescriptorFactory.Find(item?.GetType() ?? descriptor.ElementType));
            CurrentPath.Pop();
            i++;
        }
    }

    /// <inheritdoc />
    public virtual void VisitCollectionItem(IEnumerable collection, CollectionDescriptor descriptor, int index, object? item, ITypeDescriptor? itemDescriptor)
    {
        Visit(item, itemDescriptor);
    }

    /// <inheritdoc />
    public virtual void VisitDictionary(object dictionary, DictionaryDescriptor descriptor)
    {
        // Make a copy in case VisitCollectionItem mutates something
        var items = descriptor.GetEnumerator(dictionary).ToList();

        foreach (var keyValue in items)
        {
            var key = keyValue.Key!;
            var keyDescriptor = TypeDescriptorFactory.Find(keyValue.Key?.GetType() ?? descriptor.KeyType);
            var value = keyValue.Value;
            var valueDescriptor = TypeDescriptorFactory.Find(keyValue.Value?.GetType() ?? descriptor.ValueType);

            CurrentPath.Push(descriptor, key);
            VisitDictionaryKeyValue(dictionary, descriptor, key, keyDescriptor, value, valueDescriptor);
            CurrentPath.Pop();
        }
    }

    /// <inheritdoc />
    public virtual void VisitDictionaryKeyValue(object dictionary, DictionaryDescriptor descriptor, object key, ITypeDescriptor? keyDescriptor, object? value, ITypeDescriptor? valueDescriptor)
    {
        Visit(key, keyDescriptor);
        Visit(value, valueDescriptor);
    }

    /// <inheritdoc />
    public virtual void VisitSet(IEnumerable set, SetDescriptor descriptor)
    {
        // Make a copy in case VisitCollectionItem mutates something
        IEnumerator enumerator = set.GetEnumerator();
        while (enumerator.MoveNext())
        {
            object item = enumerator.Current;
            CurrentPath.Push(descriptor, item);
            VisitSetItem(set, descriptor, item, TypeDescriptorFactory.Find(item?.GetType() ?? descriptor.ElementType));
            CurrentPath.Pop();
        }
    }

    /// <inheritdoc />
    public virtual void VisitSetItem(IEnumerable set, SetDescriptor descriptor, object? item, ITypeDescriptor? itemDescriptor)
    {
        Visit(item, itemDescriptor);
    }

    protected virtual bool CanVisit(object obj)
    {
        // Always visit valuetypes
        if (obj.GetType().GetTypeInfo().IsValueType)
        {
            return true;
        }

        // true if not visited yet; otherwise, false
        return visitedObjects.Add(obj);
    }
}
