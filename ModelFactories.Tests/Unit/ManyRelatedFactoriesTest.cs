using FluentAssertions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class ManyRelatedFactoriesTest
{
    [Fact]
    public void ItCreatesManyRelated()
    {
        var post = new PostWithManyCommentsFactory()
            .WithMany<Comment, CommentFactory>(p => p.Comments, 2)
            .Create();

        post.Comments.Should().HaveCount(2);
        post.Comments.Should().BeOfType<List<Comment>>();
        post.Comments.ForEach(p => p.Text.Should().Be("Some text"));
    }

    [Fact]
    public void ItCreatesManyRelatedWithCallback()
    {
        var post = new PostWithManyCommentsFactory()
            .WithMany<Comment, CommentFactory>(
                p => p.Comments,
                factory => factory.Property(c => c.Text, "foobar").CreateMany(5)
            )
            .Create();

        post.Comments.Should().HaveCount(5);
        post.Comments.Should().BeOfType<List<Comment>>();
        post.Comments.ForEach(p => p.Text.Should().Be("foobar"));
    }

    [Fact]
    public void ItCreatesManyRelatedWithSequenceCallback()
    {
        var index = 0;

        var post = new PostWithManyCommentsFactory()
            .WithMany<Comment, CommentFactory>(
                p => p.Comments,
                5,
                factory => factory.Property(
                    c => c.Text,
                    () =>
                    {
                        index++;

                        return index.ToString();
                    }
                )
            )
            .Create();

        post.Comments.Should().HaveCount(5);
        post.Comments.Should().BeOfType<List<Comment>>();

        var counter = 1;
        post.Comments.ForEach(
            p =>
            {
                p.Text.Should().Be(counter.ToString());
                counter++;
            }
        );
    }

    [Fact]
    public void ItCanUseWithManyWhenCreatingMany()
    {
        var posts = new PostWithManyCommentsFactory()
            .WithMany<Comment, CommentFactory>(
                p => p.Comments,
                2,
                factory => factory.Property(c => c.Text, "comment")
            )
            .CreateMany(2);

        foreach (var post in posts)
        {
            post.Comments.Should().HaveCount(2);
            post.Comments.Should().BeOfType<List<Comment>>();
            post.Comments.ForEach(p => p.Text.Should().Be("comment"));
        }
    }

    [Fact]
    public void ItCanUseSimpleWithManySyntaxWhenFactoriesAreMapped()
    {
        FactoryMap.DiscoverFactoriesInAssembly(GetType().Assembly);

        var post = new PostWithManyCommentsFactory()
            .WithMany<Comment>(p => p.Comments, 2)
            .Create();

        post.Comments.Should().HaveCount(2);
    }

    [Fact]
    public void ItCanUseSimpleWithManySyntaxWhenFactoriesAreMappedWithCallback()
    {
        FactoryMap.DiscoverFactoriesInAssembly(GetType().Assembly);

        var post = new PostWithManyCommentsFactory()
            .WithMany<Comment>(p => p.Comments, factory => factory.Property(x => x.Text, "Foo").CreateMany(2))
            .Create();

        post.Comments.Should().HaveCount(2);
        post.Comments.ForEach(p => p.Text.Should().Be("Foo"));
    }

    [Fact]
    public void ItCanUseSimpleWithManySyntaxWhenFactoriesAreMappedWithFactoryCallback()
    {
        FactoryMap.DiscoverFactoriesInAssembly(GetType().Assembly);

        var post = new PostWithManyCommentsFactory()
            .WithMany<Comment>(p => p.Comments, 2, factory => factory.Property(x => x.Text, "Bar"))
            .Create();

        post.Comments.Should().HaveCount(2);
        post.Comments.ForEach(p => p.Text.Should().Be("Bar"));
    }
}
