using FluentAssertions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class RecyclingTest
{
    [Fact]
    public void ItRecyclesValue()
    {
        var author = new AuthorFactory().Create();

        var post = new PostFactory()
            .Recycle(author)
            .Create();

        post.Author.Should().Be(author);
    }

    [Fact]
    public void ItRecyclesDeeply()
    {
        var author = new AuthorFactory().Create();

        var blog = new BlogFactory()
            .WithMany<Post, PostFactory>(b => b.Posts, 5)
            .Recycle(author)
            .Create();

        blog.Posts.Should().HaveCount(5);
        blog.Posts.ForEach(p => p.Author.Should().Be(author));
    }
}
