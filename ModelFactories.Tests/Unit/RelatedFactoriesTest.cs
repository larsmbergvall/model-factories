using System.Reflection;
using FluentAssertions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class RelatedFactoriesTest
{
    [Fact]
    public void ItCreatesRelatedModelsUsingFactory()
    {
        var post = new PostFactory()
            .With<Author, AuthorFactory>(p => p.Author)
            .Create();

        post.Author.Should().NotBeNull();
        post.Author.Should().BeOfType(typeof(Author));
    }

    [Fact]
    public void ItCreatesRelatedModelsUsingFactoryWithState()
    {
        var post = new PostFactory()
            .With<Author, AuthorFactory>(
                p => p.Author,
                f => f.Property(a => a.Name, () => "::author name::")
                    .Create()
            )
            .Create();

        post.Author.Should().NotBeNull();
        post.Author.Should().BeOfType(typeof(Author));
        post.Author!.Name.Should().Be("::author name::");
    }

    [Fact]
    public void ItCreatesRelatedModelsForAllWhenCreatingMany()
    {
        var posts = new PostFactory()
            .With<Author, AuthorFactory>(
                p => p.Author,
                f => f.Property(a => a.Name, () => "::author name::")
                    .Create()
            )
            .CreateMany(2);

        posts.Should().BeOfType<List<Post>>();
        posts.Should().HaveCount(2);

        posts.ForEach(
            post =>
            {
                post.Author.Should().NotBeNull();
                post.Author.Should().BeOfType(typeof(Author));
                post.Author!.Name.Should().Be("::author name::");
            }
        );
    }

    [Fact]
    public void ItCanOverrideRelatedWithProperty()
    {
        var post = new PostFactory()
            .Property(a => a.Author, () => null)
            .Create();

        post.Author.Should().BeNull();
    }

    [Fact]
    public void ItCanUseSimpleWithSyntaxWhenFactoriesAreMapped()
    {
        FactoryMap.DiscoverFactoriesInAssembly(Assembly.GetExecutingAssembly());

        var post = new PostFactory()
            .With<Author>(p => p.Author)
            .Create();

        post.Author.Should().NotBeNull();
    }

    [Fact]
    public void ItCanUseSimpleWithSyntaxWithStateWhenFactoriesAreMapped()
    {
        FactoryMap.DiscoverFactoriesInAssembly(Assembly.GetExecutingAssembly());

        var post = new PostFactory()
            .With<Author>(
                p => p.Author,
                factory => factory.Property(a => a.Name, () => "::author name::").Create()
            )
            .Create();

        post.Author.Should().NotBeNull();
    }
}
