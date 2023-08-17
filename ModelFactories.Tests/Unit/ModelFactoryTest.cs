using ModelFactories.Tests.Models;
using FluentAssertions;

namespace ModelFactories.Tests.Unit;

public class ModelFactoryTest
{
    [Fact]
    public void ItCanCreateAModel()
    {
        var post = new PostFactory().Create();

        post.Id.Should().NotBeEmpty();
        post.Title.Should().NotBeEmpty();
        post.Body.Should().NotBeEmpty();
        post.CreatedAt.Should().NotBe(null);
    }

    [Fact]
    public void ItCanOverrideASinglePropWhenCreatingOne()
    {
        var post = new PostFactory()
            .Create((x => x.Title, f => "Foo"));

        post.Title.Should().Be("Foo");
    }

    [Fact]
    public void ItCanOverrideMultiplePropsWhenCreatingOne()
    {
        var post = new PostFactory()
            .Create(
                (x => x.Title, f => "::title::"),
                (x => x.Body, f => "::body::")
            );


        post.Title.Should().Be("::title::");
        post.Body.Should().Be("::body::");
    }

    [Fact]
    public void ItCanOverrideASinglePropWhenCreatingMany()
    {
        var posts = new PostFactory()
            .CreateMany(2, (x => x.Title, f => "Bar"));

        posts.ForEach(x => x.Title.Should().Be("Bar"));
    }

    [Fact]
    public void ItCanOverrideMultiplePropsWhenCreatingMany()
    {
        var posts = new PostFactory()
            .CreateMany(2,
                (x => x.Title, f => "::title::"),
                (x => x.Body, f => "::body::")
            );

        posts.ForEach(x =>
        {
            x.Title.Should().Be("::title::");
            x.Body.Should().Be("::body::");
        });
    }

    [Fact]
    public void ItCanOverrideMultiplePropsWhenCreatingManyWithCreateMethod()
    {
        var posts = new PostFactory()
            .Create(2,
                (x => x.Title, f => "::title::"),
                (x => x.Body, f => "::body::")
            );

        posts.ForEach(x =>
        {
            x.Title.Should().Be("::title::");
            x.Body.Should().Be("::body::");
        });
    }

    [Fact]
    public void ItCanUseStates()
    {
        var post = new PostFactory().WithTestTitle().Create();

        post.Title.Should().Be("Test");
    }

    [Fact]
    public void ItCanUseMultipleStates()
    {
        var post = new PostFactory()
            .WithTestTitle()
            .WithTestBody()
            .Create();

        post.Title.Should().Be("Test");
        post.Body.Should().Be("Test");
    }

    [Fact]
    public void ItCreatesRelatedModelsUsingFactory()
    {
        var post = new PostFactory().WithAuthor().Create();

        post.Author.Should().NotBeNull();
        post.Author.Should().BeOfType(typeof(Author));
    }

    [Fact]
    public void ItCreatesRelatedModelsUsingFactoryWithState()
    {
        var post = new PostFactory().WithAuthorWithTestName().Create();

        post.Author.Should().NotBeNull();
        post.Author.Should().BeOfType(typeof(Author));
        post.Author!.Name.Should().Be("Test Name");
    }
}
