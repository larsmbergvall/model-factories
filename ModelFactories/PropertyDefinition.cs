using System.Linq.Expressions;

namespace ModelFactories;

public interface IPropertyDefinition
{
    string PropertyName { get; }
    Delegate Callback { get; }
}

public class PropertyDefinition<T> : IPropertyDefinition
{
    public string PropertyName { get; private set; }
    public Func<T> Callback { get; private set; }

    Delegate IPropertyDefinition.Callback => Callback;

    public PropertyDefinition(string propertyName, Func<T> callback)
    {
        PropertyName = propertyName;
        Callback = callback;
    }
}

// public record PropertyDefinition<T>(string PropertyName, Func<T> Callback);
// public record PropertyDefinitionWithModel<T, TModel>(string PropertyName, Func<TModel, T> Callback) where TModel : class;
// public class PropertyDefinitionWithModel<T, TModel> where TModel : class
// {
//     private readonly string _propertyName;
//     private Func<TModel, T?> _callback;
//
//     public PropertyDefinitionWithModel(string propertyName, Func<TModel, T?> callback)
//     {
//         _propertyName = propertyName;
//         _callback = callback;
//     }
// }
//
// public class PropertyDefinition<T>
// {
//     private readonly string _propertyName;
//     private Func<T> _callback;
//
//     public PropertyDefinition(string propertyName, Func<T> callback)
//     {
//         _propertyName = propertyName;
//         _callback = callback;
//     }
// }
