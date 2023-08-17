using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class NewAuthorFactory : NewModelFactory<Author>
{
    protected override void Definition()
    {
        Property(a => a.Name, () => "foo");
    }
}
