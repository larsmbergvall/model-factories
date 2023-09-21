using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class PostWithManyCommentsFactory : ModelFactory<PostWithManyComments>
{
    private static int idCounter = 0;

    protected override void Definition()
    {
        Property(p => p.Title, "::post title::")
            .Property(p => p.Id, () => idCounter);

        idCounter++;
    }
}