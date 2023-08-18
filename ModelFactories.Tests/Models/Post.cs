namespace ModelFactories.Tests.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? PublishedFrom { get; set; } = null;

    public Author? Author { get; set; }
}