namespace ModelFactories;

public interface IRelatedDefinition
{
    string PropertyName { get; }
    Delegate Callback { get; }

    Delegate CreateFactory { get; }
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