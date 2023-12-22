namespace ModelFactories;

public abstract partial class ModelFactory<T> where T : class, new()
{
    public ModelFactory<T> AfterCreate(Func<T, T> callback)
    {
        _afterCallbacks.Add(callback);

        return this;
    }

    private T ExecuteAfterCallbacks(T model)
    {
        foreach (var callback in _afterCallbacks)
        {
            model = callback(model);
        }

        return model;
    }
}
