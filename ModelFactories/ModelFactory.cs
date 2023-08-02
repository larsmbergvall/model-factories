using System.Linq.Expressions;
using Bogus;

namespace ModelFactories;

public abstract class ModelFactory<T> : Faker<T> where T : class
{
    private int _count = 1;
    private bool _isConfigured = false;

    public ModelFactory()
    {
        EnsureConfigured();
    }

    protected abstract void Definition();

    public ModelFactory<T> Count(int count)
    {
        _count = count;

        return this;
    }

    protected ModelFactory<T> State<TProperty>(
        params (Expression<Func<T, TProperty>>, Func<Faker, TProperty>)[] attributes)
    {
        foreach (var (property, setter) in attributes)
        {
            RuleFor(property, setter);
        }

        return this;
    }

    /// <summary>
    /// Create a nested/related model using a factory
    /// </summary>
    /// <param name="property">Property to set, i.e. post => post.Author</param>
    /// <typeparam name="TRelated">Related type, i.e. Author</typeparam>
    /// <typeparam name="TFactory">Related type factory, i.e. AuthorFactory</typeparam>
    /// <returns></returns>
    protected ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, object?>> property)
        where TRelated: class
        where TFactory: ModelFactory<TRelated>, new()
    {
        var factory = new TFactory();

        RuleFor(property, _ => factory.Create());

        return this;
    }
    protected ModelFactory<T> With<TRelated, TFactory>(
        Expression<Func<T, object?>> property,
        params (Expression<Func<TRelated, object?>>, Func<Faker, object?>)[] attributes
        )
        where TRelated: class
        where TFactory: ModelFactory<TRelated>, new()
    {
        var factory = new TFactory();

        factory.State(attributes);

        RuleFor(property, _ => factory.Create());

        return this;
    }

    private void EnsureConfigured()
    {
        if (!_isConfigured)
        {
            Definition();
            _isConfigured = true;
        }
    }

    # region Create Single

    public T Create()
    {
        return Generate();
    }

    public T Create<TProperty>(
        params (Expression<Func<T, TProperty>> property, Func<Faker, TProperty> setter)[] attributes)
    {
        foreach (var (property, setter) in attributes)
        {
            RuleFor(property, setter);
        }

        return Create();
    }

    #endregion

    #region Create Many

    public List<T> Create(int count)
    {
        _count = count;

        return CreateMany();
    }

    public List<T> CreateMany()
    {
        var createdModels = new List<T>();

        for (int i = 0; i < _count; i++)
        {
            createdModels.Add(Create());
        }

        return createdModels;
    }

    public List<T> CreateMany<TProperty>(int count,
        params (Expression<Func<T, TProperty>>, Func<Faker, TProperty>)[] attributes)
    {
        EnsureConfigured();
        _count = count;

        foreach (var (property, setter) in attributes)
        {
            RuleFor(property, setter);
        }

        return CreateMany();
    }


    public List<T> Create<TProperty>(int count,
        params (Expression<Func<T, TProperty>>, Func<Faker, TProperty>)[] attributes)
    {
        return CreateMany(count, attributes);
    }

    #endregion
}
