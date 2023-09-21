using FluentAssertions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class ManyRelatedFactoriesTest
{
    [Fact]
    public void ItCreatesManyRelated()
    {
        PostWithManyComments post = new PostWithManyCommentsFactory()
            .WithMany<Comment, CommentFactory>(2)
            .Create();

        post.Comments.Should().HaveCount(2);
        post.Comments.Should().BeOfType<List<Comment>>();
        post.Comments.ForEach(p => p.Text.Should().Be("Some text"));
    }
}
