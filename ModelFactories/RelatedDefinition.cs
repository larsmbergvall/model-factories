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
    public ManyRelatedDefinition(string propertyName, uint count, Func<TFactory, ModelFactory<TModel>> callback)
    {
        PropertyName = propertyName;
        Count = count;
        Callback = factory => callback(factory).CreateMany(count);
    }

    public ManyRelatedDefinition(string propertyName, Func<TFactory, List<TModel>> callback)
    {
        PropertyName = propertyName;
        Callback = callback;
    }

    public ManyRelatedDefinition(string propertyName, uint count)
    {
        PropertyName = propertyName;
        Count = count;
        Callback = factory => factory.CreateMany(Count);
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
    public RelatedDefinition(string propertyName, Func<TFactory, TModel>? callback = null)
    {
        PropertyName = propertyName;

        Callback = callback ?? (factory => factory.Create());
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
