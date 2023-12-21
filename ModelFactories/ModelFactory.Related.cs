using System.Linq.Expressions;

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
}
