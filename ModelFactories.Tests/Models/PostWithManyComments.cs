namespace ModelFactories.Tests.Models;

public class PostWithManyComments
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = String.Empty;
    public List<Comment> Comments { get; set; } = new();
}