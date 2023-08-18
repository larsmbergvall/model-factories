using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class NewPostFactory : NewModelFactory<Post>
{
    protected override void Definition()
    {
        Property(p => p.Id, () => Guid.NewGuid())
            .Property(p => p.Title, () => "Post title")
            .Property(p => p.Body, () => "Lorem ipsum")
            .Property(p => p.CreatedAt, () => DateTime.Now);
    }
}
