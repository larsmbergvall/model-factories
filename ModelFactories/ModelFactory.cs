using System.Linq.Expressions;
using System.Reflection;
using ModelFactories.Exceptions;

namespace ModelFactories;

public abstract partial class ModelFactory<T> where T : class, new()
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
}
