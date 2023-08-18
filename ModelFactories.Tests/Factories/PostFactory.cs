using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class PostFactory : ModelFactory<Post>
{
    protected override void Definition()
    {
        Property(p => p.Id, () => Guid.NewGuid())
            .Property(p => p.Title, () => "Post title")
            .Property(p => p.Body, () => "Lorem ipsum")
            .Property(p => p.CreatedAt, () => DateTime.Now);
    }

    public PostFactory Published()
    {
        Property(p => p.PublishedFrom, () => DateTime.Now);

        return this;
    }

    public PostFactory WithFooTitle()
    {
        Property(p => p.Title, () => "foo");

        return this;
    }
}