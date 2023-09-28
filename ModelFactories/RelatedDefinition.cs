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
