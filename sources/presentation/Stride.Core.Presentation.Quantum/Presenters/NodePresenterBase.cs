// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Extensions;
using Stride.Core.Reflection;
using Stride.Core.Quantum;

namespace Stride.Core.Presentation.Quantum.Presenters;

public abstract class NodePresenterBase : IInitializingNodePresenter
{
    private readonly INodePresenterFactoryInternal factory;
    private readonly List<INodePresenter> children = [];
    private HashSet<INodePresenter>? dependencies;

    protected NodePresenterBase(INodePresenterFactoryInternal factory, IPropertyProviderViewModel? propertyProvider, INodePresenter? parent)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        Parent = parent;
        PropertyProvider = propertyProvider;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    dependency.ValueChanged -= DependencyChanged;
                }
            }
        }
    }

    public INodePresenter this[string childName] => TryGetChild(childName) ?? throw new KeyNotFoundException($"Key {childName} not found in {nameof(INodePresenter)}");

    public INodePresenter Root => Parent?.Root ?? this;

    public INodePresenter? Parent { get; private set; }

    public IReadOnlyList<INodePresenter> Children => children;

    public string DisplayName { get; set; }

    public string Name { get; protected set; }

    public List<INodePresenterCommand> Commands { get; } = [];

    public abstract Type Type { get; }

    public abstract bool IsEnumerable { get; }

    public bool IsVisible { get; set; } = true;

    public bool IsReadOnly { get; set; }

    public int? Order { get; set; }

    public abstract NodeIndex Index { get; }

    public abstract ITypeDescriptor? Descriptor { get; }

    public abstract object Value { get; }

    public string CombineKey { get; set; }

    public PropertyContainerClass AttachedProperties { get; } = [];

    public event EventHandler<ValueChangingEventArgs>? ValueChanging;

    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    protected abstract IObjectNode? ParentingNode { get; }

    public abstract void UpdateValue(object newValue);

    public abstract void AddItem(object value);

    public abstract void AddItem(object value, NodeIndex index);

    public abstract void RemoveItem(object value, NodeIndex index);

    public abstract NodeAccessor GetNodeAccessor();

    public IPropertyProviderViewModel? PropertyProvider { get; }

    public INodePresenterFactory Factory => factory;

    public override string ToString()
    {
        return $"[{GetType().Name}] {Name} (Count = {Children.Count}";
    }

    public void ChangeParent(INodePresenter newParent)
    {
        ArgumentNullException.ThrowIfNull(newParent);

        var parent = (NodePresenterBase?)Parent;
        parent?.children.Remove(this);

        parent = (NodePresenterBase)newParent;
        parent.children.Add(this);

        Parent = newParent;
    }

    public void Rename(string newName, bool overwriteCombineKey = true)
    {
        Name = newName;
        if (overwriteCombineKey)
        {
            CombineKey = newName;
        }
    }

    public INodePresenter? TryGetChild(string childName)
    {
        return children.FirstOrDefault(x => string.Equals(x.Name, childName, StringComparison.Ordinal));
    }

    public void AddDependency(INodePresenter node, bool refreshOnNestedNodeChanges)
    {
        ArgumentNullException.ThrowIfNull(node);

        dependencies ??= [];
        if (dependencies.Add(node))
        {
            node.ValueChanged += DependencyChanged;
        }
    }

    protected void Refresh()
    {
        // Remove existing children and attached properties
        foreach (var child in children.DepthFirst(x => x.Children))
        {
            child.Dispose();
        }
        children.Clear();
        AttachedProperties.Clear();

        // And recompute them from the current value.
        factory.CreateChildren(this, ParentingNode, PropertyProvider);
    }

    protected void AttachCommands()
    {
        foreach (var command in factory.AvailableCommands)
        {
            if (command.CanAttach(this))
                Commands.Add(command);
        }
    }

    protected void RaiseValueChanging(object? newValue)
    {
        ValueChanging?.Invoke(this, new ValueChangingEventArgs(newValue));
    }

    protected void RaiseValueChanged(object? oldValue)
    {
        ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue));
    }

    private void DependencyChanged(object? sender, ValueChangedEventArgs e)
    {
        RaiseValueChanging(Value);
        Refresh();
        RaiseValueChanged(Value);
    }

    void IInitializingNodePresenter.AddChild(IInitializingNodePresenter child)
    {
        children.Add(child);
    }

    void IInitializingNodePresenter.FinalizeInitialization()
    {
    }
}
