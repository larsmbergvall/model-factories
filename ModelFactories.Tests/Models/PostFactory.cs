
namespace ModelFactories.Tests.Models;

public class PostFactory : ModelFactory<Post>
{
    public PostFactory WithTestTitle()
    {
        return (PostFactory)State(
            (x => x.Title, f => "Test")
        );
    }

    public PostFactory WithTestBody()
    {
        return (PostFactory)State(
            (x => x.Body, f => "Test")
        );
    }

    protected override void Definition()
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Title, f => string.Join(' ', f.Lorem.Words(5)));
        RuleFor(x => x.Body, f => string.Join('\n', f.Lorem.Paragraphs(
            f.Random.UShort(3, 10))
        ));
        RuleFor(
            x => x.CreatedAt,
            f => f.Date.Between(DateTime.Today.AddYears(-2), DateTime.Today)
        );
    }
}