using System.Linq.Expressions;
using System.Reflection;
using ModelFactories.Exceptions;

namespace ModelFactories;

public abstract class ModelFactory<T> where T : class, new()
{
	private Dictionary<string, IPropertyDefinition> _definitions = new();
	private Dictionary<string, IPropertyDefinition> _definitionsWithModel = new();
	private T? _model = null;
	private Dictionary<string, IRelatedDefinition> _relatedFactories = new();
	private List<Func<T, T>> _afterCallbacks = new();

	public ModelFactory()
	{
		Configure();
	}

	protected abstract void Definition();

	public T Create()
	{
		foreach (var (key, prop) in _definitions)
		{
			ApplyProperty(prop);
		}

		foreach (var (key, prop) in _definitionsWithModel)
		{
			ApplyProperty(prop, true);
		}

		foreach (var (key, related) in _relatedFactories)
		{
			CreateRelated(related);
		}

		return ExecuteAfterCallbacks(GetModel());
	}

	public List<T> Create(uint count)
	{
		return CreateMany(count);
	}

	public List<T> CreateMany(uint count = 1)
	{
		var list = new List<T>();

		for (int i = 0; i < count; i++)
		{
			list.Add(ExecuteAfterCallbacks(Create()));
		}

		return list;
	}

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

	public ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, TRelated?>> property)
		where TRelated : class, new()
		where TFactory : ModelFactory<TRelated>, new()
	{
		var propertyName = PropertyName(property);
		RemovePropertyKeysIfExists(propertyName);

		_relatedFactories.Add(
			propertyName,
			new RelatedDefinition<TRelated, TFactory>(propertyName)
		);

		return this;
	}

	public ModelFactory<T> With<TRelated, TFactory>(Expression<Func<T, TRelated?>> property,
		Func<TFactory, TRelated> callback)
		where TRelated : class, new()
		where TFactory : ModelFactory<TRelated>, new()
	{
		var propertyName = PropertyName(property);
		RemovePropertyKeysIfExists(propertyName);

		_relatedFactories.Add(
			propertyName,
			new RelatedDefinition<TRelated, TFactory>(propertyName, callback)
		);

		return this;
	}

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


	private T GetModel()
	{
		if (_model is null)
		{
			_model = new T();
		}

		return _model;
	}

	private void ApplyProperty(IPropertyDefinition propertyDefinition, bool withModel = false)
	{
		var model = GetModel();
		var reflection = model.GetType();
		var prop = reflection.GetProperty(propertyDefinition.PropertyName);

		EnsurePropExistsAndIsWritable(propertyDefinition, prop, reflection);

		if (withModel)
		{
			prop!.SetValue(model, propertyDefinition.Callback.DynamicInvoke(model));
			return;
		}

		prop!.SetValue(model, propertyDefinition.Callback.DynamicInvoke());
	}

	private void CreateRelated(IRelatedDefinition relatedDefinition)
	{
		var model = GetModel();
		var reflection = model.GetType();
		var prop = reflection.GetProperty(relatedDefinition.PropertyName);

		EnsurePropExistsAndIsWritable(relatedDefinition, prop, reflection);

		prop!.SetValue(
			model,
			relatedDefinition.Callback.DynamicInvoke(relatedDefinition.CreateFactory.DynamicInvoke())
		);
	}

	private void Configure()
	{
		Definition();
	}

	/// <summary>
	/// Removes a key from both definition dictionaries. This is to ensure there is only ever one
	/// definition for a given property
	/// </summary>
	/// <param name="key"></param>
	private void RemovePropertyKeysIfExists(string key)
	{
		_definitions.Remove(key);
		_definitionsWithModel.Remove(key);
		_relatedFactories.Remove(key);
	}

	private static void EnsurePropExistsAndIsWritable(IPropertyDefinition definition, PropertyInfo? propInfo, Type type)
	{
		if (propInfo is null)
		{
			throw new ModelFactoryException($"Property {definition.PropertyName} does not exist on {type.FullName}");
		}

		if (!propInfo.CanWrite)
		{
			throw new PropIsReadOnlyException(definition, type);
		}
	}

	private static void EnsurePropExistsAndIsWritable(IRelatedDefinition definition, PropertyInfo? propInfo, Type type)
	{
		if (propInfo is null)
		{
			throw new ModelFactoryException($"Property {definition.PropertyName} does not exist on {type.FullName}");
		}

		if (!propInfo.CanWrite)
		{
			throw new PropIsReadOnlyException(definition, type);
		}
	}

	private static string PropertyName<TType, TReturn>(Expression<Func<TType, TReturn>> expression)
	{
		if (expression.Body is MemberExpression memberExpression)
		{
			return memberExpression.Member.Name;
		}

		throw new ModelFactoryException("Could not extract name from expression");
	}
}
