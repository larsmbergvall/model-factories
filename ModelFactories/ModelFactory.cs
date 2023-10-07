using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ModelFactories.Exceptions;

[assembly: InternalsVisibleTo("ModelFactories.Tests")]

namespace ModelFactories;

public abstract class ModelFactory<T> where T : class, new()
{
    private List<Func<T, T>> _afterCallbacks = new();
    private Dictionary<string, IPropertyDefinition> _definitions = new();
    private Dictionary<string, IPropertyDefinition> _definitionsWithModel = new();
    private Dictionary<string, object> _recycledObjects = new();
    private Dictionary<string, IRelatedDefinition> _relatedFactories = new();

    public ModelFactory()
    {
        Configure();
    }

    protected abstract void Definition();

    public T Create()
    {
        var model = new T();

        foreach (var (key, prop) in _definitions)
        {
            ApplyProperty(model, prop);
        }

        foreach (var (key, prop) in _definitionsWithModel)
        {
            ApplyProperty(model, prop, true);
        }

        foreach (var (key, related) in _relatedFactories)
        {
            CreateRelated(model, related);
        }

        return ExecuteAfterCallbacks(model);
    }

    public List<T> Create(uint count)
    {
        return CreateMany(count);
    }

    public List<T> CreateMany(uint count = 1)
    {
        var list = new List<T>();

        for (var i = 0; i < count; i++)
        {
            list.Add(Create());
        }

        return list;
    }

    #region Hooks

    public ModelFactory<T> AfterCreate(Func<T, T> callback)
    {
        _afterCallbacks.Add(callback);

        return this;
    }

    #endregion

    private T ExecuteAfterCallbacks(T model)
    {
        foreach (var callback in _afterCallbacks)
        {
            model = callback(model);
        }

        return model;
    }

    private bool WasRecycled(PropertyInfo prop, T model)
    {
        if (_recycledObjects.TryGetValue(prop.PropertyType.FullName!, out var recycled))
        {
            prop!.SetValue(model, recycled);
            return true;
        }

        return false;
    }

    private void ApplyProperty(T model, IPropertyDefinition propertyDefinition, bool withModel = false)
    {
        var reflection = model.GetType();
        var prop = reflection.GetProperty(propertyDefinition.PropertyName);

        EnsurePropExistsAndIsWritable(propertyDefinition, prop, reflection);

        // Recycled values take priority over regular definitions
        if (WasRecycled(prop!, model))
        {
            return;
        }

        if (withModel)
        {
            prop!.SetValue(model, propertyDefinition.Callback.DynamicInvoke(model));
            return;
        }

        prop!.SetValue(model, propertyDefinition.Callback.DynamicInvoke());
    }

    private void CreateRelated(T model, IRelatedDefinition relatedDefinition)
    {
        var reflection = model.GetType();
        var prop = reflection.GetProperty(relatedDefinition.PropertyName);

        EnsurePropExistsAndIsWritable(relatedDefinition, prop, reflection);

        // Recycled values take priority over regular definitions
        if (WasRecycled(prop!, model))
        {
            return;
        }

        prop!.SetValue(
            model,
            relatedDefinition.Callback.DynamicInvoke(relatedDefinition.CreateFactory.DynamicInvoke())
        );
    }

    private void Configure()
    {
        Definition();
    }

    /// <summary>
    /// Removes a key from both definition dictionaries. This is to ensure there is only ever one
    /// definition for a given property
    /// </summary>
    /// <param name="key"></param>
    private void RemovePropertyKeysIfExists(string key)
    {
        _definitions.Remove(key);
        _definitionsWithModel.Remove(key);
        _relatedFactories.Remove(key);
    }

    private static void EnsurePropExistsAndIsWritable(IPropertyDefinition definition, PropertyInfo? propInfo, Type type)
    {
        if (propInfo is null)
        {
            throw new ModelFactoryException($"Property {definition.PropertyName} does not exist on {type.FullName}");
        }

        if (!propInfo.CanWrite)
        {
            throw new PropIsReadOnlyException(definition, type);
        }
    }

    private static void EnsurePropExistsAndIsWritable(IRelatedDefinition definition, PropertyInfo? propInfo, Type type)
    {
        if (propInfo is null)
        {
            throw new ModelFactoryException($"Property {definition.PropertyName} does not exist on {type.FullName}");
        }

        if (!propInfo.CanWrite)
        {
            throw new PropIsReadOnlyException(definition, type);
        }
    }

    private static string PropertyName<TType, TReturn>(Expression<Func<TType, TReturn>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ModelFactoryException("Could not extract name from expression");
    }

    #region Recycling

    public ModelFactory<T> Recycle<TModel>(TModel recycledModel)
    {
        if (recycledModel is null)
        {
            throw new ModelFactoryException("Cannot recycle null");
        }

        _recycledObjects.TryAdd(typeof(TModel).FullName!, recycledModel);

        return this;
    }

    internal ModelFactory<T> SetRecycledObjects(Dictionary<string, object> recycledObjects)
    {
        _recycledObjects = recycledObjects;

        return this;
    }

    #endregion

    #region Property

    public ModelFactory<T> Property<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<TProperty> callback
    )
    {
        var propertyName = PropertyName(propertyExpression);
        RemovePropertyKeysIfExists(propertyName);
        _definitions.Add(propertyName, new PropertyDefinition<TProperty>(propertyName, callback));

        return this;
    }

    public ModelFactory<T> Property<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<T, TProperty> callback
    )
    {
        var propertyName = PropertyName(propertyExpression);
        RemovePropertyKeysIfExists(propertyName);
        _definitionsWithModel.Add(propertyName, new PropertyDefinitionWithModel<T, TProperty>(propertyName, callback));

        return this;
    }

    public ModelFactory<T> Property<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        TProperty value
    )
    {
        var propertyName = PropertyName(propertyExpression);
        RemovePropertyKeysIfExists(propertyName);
        _definitions.Add(propertyName, new PropertyDefinition<TProperty>(propertyName, () => value));

        return this;
    }

    #endregion

    #region Related Factories

    public ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, TRelated?>> property)
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        var propertyName = PropertyName(property);
        RemovePropertyKeysIfExists(propertyName);

        _relatedFactories.Add(
            propertyName,
            new RelatedDefinition<TRelated, TFactory>(propertyName, _recycledObjects)
        );

        return this;
    }

    public ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, TRelated?>> property,
        Func<TFactory, TRelated> callback
    )
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        var propertyName = PropertyName(property);
        RemovePropertyKeysIfExists(propertyName);

        _relatedFactories.Add(
            propertyName,
            new RelatedDefinition<TRelated, TFactory>(propertyName, _recycledObjects, callback)
        );

        return this;
    }

    public ModelFactory<T> WithMany<TRelated, TFactory>(Expression<Func<T, List<TRelated>>> property,
        uint count
    )
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        var propertyName = PropertyName(property);
        RemovePropertyKeysIfExists(propertyName);

        _relatedFactories.Add(
            propertyName,
            new ManyRelatedDefinition<TRelated, TFactory>(propertyName, count, _recycledObjects)
        );

        return this;
    }

    public ModelFactory<T> WithMany<TRelated, TFactory>(Expression<Func<T, List<TRelated>>> property,
        Func<TFactory, List<TRelated>> callback
    )
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        var propertyName = PropertyName(property);
        RemovePropertyKeysIfExists(propertyName);

        _relatedFactories.Add(
            propertyName,
            new ManyRelatedDefinition<TRelated, TFactory>(propertyName, _recycledObjects, callback)
        );

        return this;
    }

    public ModelFactory<T> WithMany<TRelated, TFactory>(Expression<Func<T, List<TRelated>>> property,
        uint count,
        Func<TFactory, ModelFactory<TRelated>> callback
    )
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        var propertyName = PropertyName(property);
        RemovePropertyKeysIfExists(propertyName);

        _relatedFactories.Add(
            propertyName,
            new ManyRelatedDefinition<TRelated, TFactory>(propertyName, count, _recycledObjects, callback)
        );

        return this;
    }

    #endregion
}
