using System.Linq.Expressions;
using System.Reflection;
using ModelFactories.Exceptions;

namespace ModelFactories;

public abstract partial class ModelFactory<T> where T : class, new()
{
    /// <summary>
    /// This method is used to specify that a property should be generated using a ModelFactory
    /// </summary>
    /// <param name="property"></param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <typeparam name="TFactory">ModelFactory that will create related class</typeparam>
    /// <returns></returns>
    public ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, TRelated?>> property)
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        return With<TRelated, TFactory>(property, null);
    }

    /// <summary>
    /// This method is used to specify that a property should be generated using a ModelFactory.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="callback">A callback where you can modify the Factory. Note that it returns a ModelFactory for the
    /// related class. You might need to cast it to access custom states.</param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <typeparam name="TFactory">ModelFactory that will create related class</typeparam>
    /// <returns></returns>
    public ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, TRelated?>> property,
        Func<TFactory, TRelated>? callback
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

    /// <summary>
    /// This method is used to specify that a property should be generated using a ModelFactory.
    /// Note that to use this, factories must be registered or auto-discovered
    /// </summary>
    /// <param name="property"></param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <returns></returns>
    public ModelFactory<T> With<TRelated>(Expression<Func<T, TRelated?>> property)
        where TRelated : class, new()
    {
        return With<TRelated>(property, null);
    }

    /// <summary>
    /// This method is used to specify that a property should be generated using a ModelFactory.
    /// Note that to use this, factories must be registered or auto-discovered
    /// </summary>
    /// <param name="property"></param>
    /// <param name="callback">A callback where you can modify the Factory. Note that it returns a ModelFactory for the
    /// related class. You might need to cast it to access custom states.</param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <returns></returns>
    public ModelFactory<T> With<TRelated>(Expression<Func<T, TRelated?>> property,
        Func<ModelFactory<TRelated>, TRelated>? callback
    )
        where TRelated : class, new()
    {
        var factory = FactoryMap.FactoryFor<TRelated>();
        var propertyName = PropertyName(property);
        RemovePropertyKeysIfExists(propertyName);

        _relatedFactories.Add(
            propertyName,
            new RelatedDefinition<TRelated>(factory, propertyName, _recycledObjects, callback)
        );

        return this;
    }

    /// <summary>
    /// This method is used to specify that a collection property should be generated using a ModelFactory.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="count">Number of items to generate</param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <typeparam name="TFactory">Factory to create items of the related class</typeparam>
    /// <returns></returns>
    public ModelFactory<T> WithMany<TRelated, TFactory>(Expression<Func<T, List<TRelated>>> property,
        uint count
    )
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        return WithMany<TRelated, TFactory>(property, factory => factory.CreateMany(count));
    }

    /// <summary>
    /// This method is used to specify that a collection property should be generated using a ModelFactory.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="callback">A callback where you can modify the Factory. Note that it returns a ModelFactory for the
    /// related class. You might need to cast it to access custom states. Also note that this should return a collection,
    /// so end it with CreateMany()</param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <typeparam name="TFactory">Factory to create items of the related class</typeparam>
    /// <returns></returns>
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

    /// <summary>
    /// This method is used to specify that a collection property should be generated using a ModelFactory.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="count">Number of items to generate</param>
    /// <param name="callback">A callback where you can modify the Factory. Note that it returns a ModelFactory for the
    /// related class. You might need to cast it to access custom states. Note that this should return the factory itself</param>
    /// <typeparam name="TRelated">The related class</typeparam>
    /// <typeparam name="TFactory">Factory to create items of the related class</typeparam>
    /// <returns></returns>
    public ModelFactory<T> WithMany<TRelated, TFactory>(Expression<Func<T, List<TRelated>>> property,
        uint count,
        Func<TFactory, ModelFactory<TRelated>> callback
    )
        where TRelated : class, new()
        where TFactory : ModelFactory<TRelated>, new()
    {
        return WithMany<TRelated, TFactory>(
            property,
            factory =>
            {
                var callbackFactory = callback(factory);
                return callbackFactory.CreateMany(count);
            }
        );
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

    #region Recycling

    /// <summary>
    /// Allows you to recycle a specific model. This means that whenever a model (even nested ones) would be generated,
    /// the model you supply here would be used instead of generating a random one.
    /// </summary>
    /// <param name="recycledModel"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    /// <exception cref="ModelFactoryException"></exception>
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

    private bool WasRecycled(PropertyInfo prop, T model)
    {
        if (_recycledObjects.TryGetValue(prop.PropertyType.FullName!, out var recycled))
        {
            prop!.SetValue(model, recycled);
            return true;
        }

        return false;
    }

    #endregion
}