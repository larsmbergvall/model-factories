namespace ModelFactories.Exceptions;

public class PropIsReadOnlyException : ModelFactoryException
{
    public PropIsReadOnlyException() : base("Property is not writable")
    {
    }

    public PropIsReadOnlyException(IPropertyDefinition prop, Type type)
        : base($"Property {prop.PropertyName} on type {type.FullName} is not writable")
    {
    }

    public PropIsReadOnlyException(IRelatedDefinition prop, Type type)
        : base($"Property {prop.PropertyName} on type {type.FullName} is not writable")
    {
    }
}