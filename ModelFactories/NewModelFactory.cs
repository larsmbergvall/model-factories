using System.Linq.Expressions;

namespace ModelFactories;

public abstract class NewModelFactory<T> where T : class, new()
{
    private List<IPropertyDefinition> _definitions = new();
    private T? _model = null;

    public NewModelFactory()
    {
        Configure();
    }

    protected abstract void Definition();

    public T Create()
    {
        foreach (var prop in _definitions)
        {
            ApplyProperty(prop);
        }

        return GetModel();
    }

    public NewModelFactory<T> Property<TProperty>(
        Expression<Func<T, TProperty>> propertyExpression,
        Func<TProperty> callback
    )
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            _definitions.Add(new PropertyDefinition<TProperty>(propertyName, callback));
        }

        return this;
    }

    private T GetModel()
    {
        if (_model is null)
        {
            _model = new T();
        }

        return _model;
    }

    private void ApplyProperty(IPropertyDefinition propertyDefinition)
    {
        var model = GetModel();
        var reflection = model.GetType();

        var prop = reflection.GetProperty(propertyDefinition.PropertyName);

        if (prop.CanWrite)
        {
            prop.SetValue(model, propertyDefinition.Callback.DynamicInvoke());
        }
    }

    private void Configure()
    {
        Definition();
    }
}
