namespace ModelFactories.Tests.Models;

public class AuthorFactory : ModelFactory<Author>
{
    protected override void Definition()
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Name, f => f.Person.FullName);
    }
}
