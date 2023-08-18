using FluentAssertions;
using ModelFactories.Exceptions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class NewModelFactoryTest
{
    #region State

    [Fact]
    public void ItWorks()
    {
        var model = new NewAuthorFactory().Create();

        model.Name.Should().Be("foo");
    }

    [Fact]
    public void ItCanOverwriteProp()
    {
        var model = new NewAuthorFactory()
            .Property(a => a.Name, () => "bar")
            .Create();

        model.Name.Should().Be("bar");
    }

    [Fact]
    public void ItCanOverwriteMultipleProps()
    {
        Guid id = Guid.NewGuid();

        var model = new NewAuthorFactory()
            .Property(a => a.Id, () => id)
            .Property(a => a.Name, () => "baz")
            .Create();

        model.Id.Should().Be(id);
        model.Name.Should().Be("baz");
    }

    [Fact]
    public void ItCanOverwritePropertyUsingModel()
    {
        var model = new NewAuthorFactory()
            .Property(a => a.Name, (model) => model.Name + "bar")
            .Create();

        model.Name.Should().Be("foobar");
    }

    [Fact]
    public void GeneratingForNonWritablePropertyThrowsException()
    {
        Assert.Throws<PropIsReadOnlyException>(() =>
        {
            new NewAuthorFactory()
                .Property(a => a.NotWritable, () => "foo")
                .Create();
        });
    }

    [Fact]
    public void ItCanUsePredefinedState()
    {
        var model = new NewPostFactory().Published().Create();

        model.PublishedFrom.Should().NotBeNull();
    }

    [Fact]
    public void ItCanCombineStateWithOverrides()
    {
        var model = new NewPostFactory()
            .Published()
            .Property(p => p.Title, () => "::title::")
            .Create();

        model.PublishedFrom.Should().NotBeNull();
        model.Title.Should().Be("::title::");
    }

    [Fact]
    public void ItCanCombineMultipleStates()
    {
        var model = new NewPostFactory()
            .Published()
            .WithFooTitle()
            .Create();

        model.PublishedFrom.Should().NotBeNull();
        model.Title.Should().Be("foo");
    }

    #endregion

    #region Related Factories

    [Fact]
    public void ItCreatesRelatedModelsUsingFactory()
    {
        var post = new NewPostFactory()
            .With<Author, NewAuthorFactory>(p => p.Author)
            .Create();

        post.Author.Should().NotBeNull();
        post.Author.Should().BeOfType(typeof(Author));
    }

    [Fact]
    public void ItCreatesRelatedModelsUsingFactoryWithState()
    {
        var post = new NewPostFactory()
            .With<Author, NewAuthorFactory>(
                p => p.Author,
                f => f.Property(a => a.Name, () => "::author name::")
                    .Create())
            .Create();

        post.Author.Should().NotBeNull();
        post.Author.Should().BeOfType(typeof(Author));
        post.Author!.Name.Should().Be("::author name::");
    }

    #endregion
}
