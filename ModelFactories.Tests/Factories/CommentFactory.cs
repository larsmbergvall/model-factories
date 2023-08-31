using ModelFactories.Tests.Models;

namespace ModelFactories.Tests.Factories;

public class CommentFactory : ModelFactory<Comment>
{
	protected override void Definition()
	{
		Property(c => c.Text, () => "Some text")
			.Property(c => c.CreatedAt, () => DateTime.Now)
			.Property(c => c.UpdatedAt, comment => comment.CreatedAt);
	}
}
