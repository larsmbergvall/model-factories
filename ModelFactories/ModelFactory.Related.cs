using System.Linq.Expressions;

namespace ModelFactories;

public abstract partial class ModelFactory<T> where T : class, new()
{
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

    /// <summary>
    /// Simple With method which does not have a factory callback
    /// </summary>
    /// <param name="property"></param>
    /// <typeparam name="TRelated"></typeparam>
    /// <returns></returns>
    public ModelFactory<T> With<TRelated>(Expression<Func<T, TRelated?>> property)
        where TRelated : class, new()
    {
        return With(property, null);
    }

    /// <summary>
    /// Simple With method that takes a Factory callback
    /// </summary>
    /// <param name="property"></param>
    /// <param name="callback"></param>
    /// <typeparam name="TRelated"></typeparam>
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
}
