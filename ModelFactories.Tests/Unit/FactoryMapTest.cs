using System.Reflection;
using FluentAssertions;
using ModelFactories.Exceptions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class FactoryMapTest
{
    public FactoryMapTest()
    {
        FactoryMap.ClearCache();
    }

    [Fact]
    public void ItDiscoversFactoriesForModels()
    {
        FactoryMap.DiscoverFactoriesInAssembly(Assembly.GetExecutingAssembly());

        var authorFactory = FactoryMap.FactoryFor<Author>();
        authorFactory.Should().NotBeNull();
        authorFactory.Should().BeOfType<AuthorFactory>();

        var commentFactory = FactoryMap.FactoryFor<Comment>();
        commentFactory.Should().NotBeNull();
        commentFactory.Should().BeOfType<CommentFactory>();
    }

    [Fact]
    public void ItCanMapManually()
    {
        FactoryMap.Map<Comment, CommentFactory>();

        var commentFactory = FactoryMap.FactoryFor<Comment>();
        commentFactory.Should().NotBeNull();
        commentFactory.Should().BeOfType<CommentFactory>();
    }

    [Fact]
    public void ItThrowsExceptionForUnmappedFactory()
    {
        Assert.Throws<ModelFactoryException>(FactoryMap.FactoryFor<Comment>);
    }
}
