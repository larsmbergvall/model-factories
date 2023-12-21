using System.Linq.Expressions;

namespace ModelFactories;

public abstract partial class ModelFactory<T> where T : class, new()
{
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
}
