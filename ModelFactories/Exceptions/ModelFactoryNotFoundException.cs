namespace ModelFactories.Exceptions;

public class ModelFactoryNotFoundException : ModelFactoryException
{
    public ModelFactoryNotFoundException(string modelName) : base($"Could not find ModelFactory for class {modelName}")
    {
    }
}