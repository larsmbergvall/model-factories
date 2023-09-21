namespace ModelFactories.Tests.Models;

public class PostWithManyComments
{
    public int Id { get; set; } = 0;
    public string Title { get; set; } = String.Empty;
    public List<Comment> Comments { get; set; } = new();
}
