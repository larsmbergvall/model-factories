using System.Reflection;
using ModelFactories.Exceptions;

namespace ModelFactories;

public static class FactoryMap
{
    internal static Dictionary<Type, Type> ModelToFactoryMap = new();

    public static ModelFactory<T> FactoryFor<T>()
        where T : class, new()
    {
        var modelType = typeof(T);

        if (ModelToFactoryMap.TryGetValue(modelType, out var factoryType))
        {
            return (ModelFactory<T>)Activator.CreateInstance(factoryType)!;
        }

        throw new ModelFactoryException($"There is no factory for type {modelType.Name}.");
    }

    public static void DiscoverFactoriesInAssembly(Assembly assembly)
    {
        var factoryTypes = assembly.GetTypes()
            .Where(type => type.IsClass
                           && type is { IsAbstract: false, BaseType.IsGenericType: true }
                           && type.BaseType.GetGenericTypeDefinition() == typeof(ModelFactory<>)
            );


        foreach (var factoryType in factoryTypes)
        {
            var genericArg = factoryType.BaseType?.GetGenericArguments().FirstOrDefault();

            if (genericArg != null)
            {
                ModelToFactoryMap.Add(genericArg, factoryType);
            }
        }
    }

    public static void Map<TModel, TFactory>()
        where TModel : class, new()
        where TFactory : ModelFactory<TModel>
    {
        ModelToFactoryMap.Add(typeof(TModel), typeof(TFactory));
    }

    internal static void ClearCache()
    {
        ModelToFactoryMap = new Dictionary<Type, Type>();
    }
}
