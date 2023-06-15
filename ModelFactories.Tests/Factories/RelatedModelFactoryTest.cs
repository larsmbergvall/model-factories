using FluentAssertions;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class RelatedModelFactoryTest
{
    [Fact]
    public void ItCanCreateRelatedModel()
    {
        var post = new PostFactoryWithAuthor().Create();

        post.Should().BeOfType<Post>();
        post.Author.Should()
            .NotBeNull()
            .And.BeOfType<Author>();
    }
}