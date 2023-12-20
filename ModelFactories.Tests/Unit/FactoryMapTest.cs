using FluentAssertions;
using ModelFactories.Exceptions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

[Collection("FactoryMapTests")]
public class FactoryMapTest
{
    public FactoryMapTest()
    {
        FactoryMap.ClearCache();
    }

    [Fact]
    public void ItDiscoversFactoriesForModels()
    {
        FactoryMap.DiscoverFactoriesInAssembly(GetType().Assembly);

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
        Assert.Throws<ModelFactoryNotFoundException>(FactoryMap.FactoryFor<Comment>);
    }

    [Fact]
    public void ItDoesNotCrashIfAutoMappingAgain()
    {
        FactoryMap.DiscoverFactoriesInAssembly(GetType().Assembly);
        FactoryMap.DiscoverFactoriesInAssembly(GetType().Assembly);

        true.Should().BeTrue();
    }

    [Fact]
    public void ItDoesNotCrashIfManuallyMappingAgain()
    {
        FactoryMap.Map<Comment, CommentFactory>();
        FactoryMap.Map<Comment, CommentFactory>();

        true.Should().BeTrue();
    }
}
