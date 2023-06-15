namespace ModelFactories.Tests.Models;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public List<Post> Posts { get; set; } = new List<Post>();
}