using FluentAssertions;
using ModelFactories.Tests.Factories;

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
}
