namespace ModelFactories;

public interface IPropertyDefinition
{
    string PropertyName { get; }
    Delegate Callback { get; }
}

public class PropertyDefinitionWithModel<TModel, T> : IPropertyDefinition where TModel : class
{
    public PropertyDefinitionWithModel(string propertyName, Func<TModel, T> callback)
    {
        PropertyName = propertyName;
        Callback = callback;
    }

    public Func<TModel, T> Callback { get; private set; }
    public string PropertyName { get; private set; }

    Delegate IPropertyDefinition.Callback => Callback;
}

public class PropertyDefinition<T> : IPropertyDefinition
{
    public PropertyDefinition(string propertyName, Func<T> callback)
    {
        PropertyName = propertyName;
        Callback = callback;
    }

    public Func<T> Callback { get; private set; }
    public string PropertyName { get; private set; }

    Delegate IPropertyDefinition.Callback => Callback;
}
