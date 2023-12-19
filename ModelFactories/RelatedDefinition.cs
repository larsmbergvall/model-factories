namespace ModelFactories;

public interface IRelatedDefinition
{
    string PropertyName { get; }
    Delegate Callback { get; }
    Delegate CreateFactory { get; }
}

public class ManyRelatedDefinition<TModel, TFactory> : IRelatedDefinition
    where TModel : class, new()
    where TFactory : ModelFactory<TModel>, new()
{
    public ManyRelatedDefinition(string propertyName, uint count, Dictionary<string, object> recycledObjects,
        Func<TFactory, ModelFactory<TModel>> callback
    )
    {
        PropertyName = propertyName;
        Count = count;
        Callback = factory => callback((TFactory)factory.SetRecycledObjects(recycledObjects)).CreateMany(count);
    }

    public ManyRelatedDefinition(string propertyName, Dictionary<string, object> recycledObjects,
        Func<TFactory, List<TModel>> callback
    )
    {
        PropertyName = propertyName;
        Callback = factory =>
        {
            factory.SetRecycledObjects(recycledObjects);

            return callback(factory);
        };
    }

    public ManyRelatedDefinition(string propertyName, uint count, Dictionary<string, object> recycledObjects)
    {
        PropertyName = propertyName;
        Count = count;
        Callback = factory => factory.SetRecycledObjects(recycledObjects).CreateMany(Count);
    }

    private uint Count { get; set; }
    private Func<TFactory, List<TModel>> Callback { get; set; }
    public string PropertyName { get; }
    Delegate IRelatedDefinition.Callback => Callback;
    Delegate IRelatedDefinition.CreateFactory => CreateFactory;

    private TFactory CreateFactory()
    {
        return new TFactory();
    }
}

/// <summary>
/// Simpler RelatedDefinition which does not use a generic factory type, but gets it's factory from the constructor
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class RelatedDefinition<TModel> : IRelatedDefinition where TModel : class, new()
{
    private ModelFactory<TModel> _factory;

    public RelatedDefinition(
        ModelFactory<TModel> modelFactory, string propertyName,
        Dictionary<string, object> recycledObjects,
        Func<ModelFactory<TModel>, TModel>? callback = null
    )
    {
        _factory = modelFactory;
        PropertyName = propertyName;

        Callback = callback is not null
            ? factory =>
            {
                factory.SetRecycledObjects(recycledObjects);
                return callback(factory);
            }
            : factory => factory.SetRecycledObjects(recycledObjects).Create();
    }

    private Func<ModelFactory<TModel>, TModel> Callback { get; set; }
    public string PropertyName { get; private set; }

    Delegate IRelatedDefinition.Callback => Callback;
    Delegate IRelatedDefinition.CreateFactory => () => _factory;
}

/// <summary>
/// RelatedDefinition which uses a generic factory type, used for the older syntax - or when not using factory discovery
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TFactory"></typeparam>
public class RelatedDefinition<TModel, TFactory> : IRelatedDefinition where TModel : class, new()
    where TFactory : ModelFactory<TModel>, new()
{
    public RelatedDefinition(string propertyName, Dictionary<string, object> recycledObjects,
        Func<TFactory, TModel>? callback = null
    )
    {
        PropertyName = propertyName;

        Callback = callback is not null
            ? factory =>
            {
                factory.SetRecycledObjects(recycledObjects);
                return callback(factory);
            }
            : factory => factory.SetRecycledObjects(recycledObjects).Create();
    }


    private Func<TFactory, TModel> Callback { get; set; }
    public string PropertyName { get; private set; }

    Delegate IRelatedDefinition.Callback => Callback;
    Delegate IRelatedDefinition.CreateFactory => CreateFactory;

    private TFactory CreateFactory()
    {
        return new TFactory();
    }
}