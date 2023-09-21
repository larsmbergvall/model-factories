using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class PostWithManyCommentsFactory : ModelFactory<PostWithManyComments>
{
    protected override void Definition()
    {
        Property(p => p.Title, "::post title::");
    }
}