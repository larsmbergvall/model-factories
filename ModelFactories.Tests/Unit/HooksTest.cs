using FluentAssertions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class HooksTest
{
    [Fact]
    public void AfterHookIsExecutedWhenCreatingOne()
    {
        var post = new PostFactory()
            .AfterCreate((Post post) =>
            {
                post.Title = "This is changed in the after creating hook";
                return post;
            })
            .Create();

        post.Title.Should().Be("This is changed in the after creating hook");
    }

    [Fact]
    public void AfterHookIsExecutedForEachItemWhenCreatingMany()
    {
        List<Post> posts = new PostFactory()
            .AfterCreate((Post post) =>
            {
                post.Title = "After";

                return post;
            })
            .CreateMany(2);

        posts.Should().HaveCount(2);
        posts.ForEach(post => post.Title.Should().Be("After"));
    }
}
