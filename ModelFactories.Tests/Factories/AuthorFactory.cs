using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class AuthorFactory : ModelFactory<Author>
{
    protected override void Definition()
    {
        Property(a => a.Name, () => "foo");
    }
}