using FluentAssertions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Unit;

public class NewModelFactoryTest
{
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
}
