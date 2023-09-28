using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class BlogFactory : ModelFactory<Blog>
{
    protected override void Definition()
    {
        Property(b => b.Name, "Le Blog");
    }
}