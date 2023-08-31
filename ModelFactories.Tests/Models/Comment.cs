namespace ModelFactories.Tests.Models;

public class Comment
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Text { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime? UpdatedAt { get; set; } = null;
}
