namespace ModelFactories.Tests.Models;

public class Blog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
}