namespace ModelFactories.Tests.Models;

public class PostFactoryWithAuthor : PostFactory
{
    protected override void Definition()
    {
        base.Definition();

        RuleFor(x => x.Author, _ => new Author());

        // BelongsTo<Author, AuthorFactory>(x => x.Author);
    }
}