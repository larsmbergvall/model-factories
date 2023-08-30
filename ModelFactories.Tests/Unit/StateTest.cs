using FluentAssertions;
using ModelFactories.Exceptions;
using ModelFactories.Tests.Factories;
using ModelFactories.Tests.Models;

namespace ModelFactories.Tests;

public class StateTest
{
	[Fact]
	public void ItCanCreateModel()
	{
		var model = new AuthorFactory().Create();

		model.Name.Should().Be("foo");
	}

	[Fact]
	public void ItCanCreateManyModels()
	{
		var models = new AuthorFactory()
			.Create(2);

		models.Should().BeOfType<List<Author>>();
		models.Should().HaveCount(2);
	}

	[Fact]
	public void ItUsesStateWhenCreatingMany()
	{
		var models = new PostFactory()
			.Published()
			.WithFooTitle()
			.CreateMany(2);

		models.Should().BeOfType<List<Post>>();
		models.Should().HaveCount(2);
		models.ForEach(p =>
		{
			p.Title.Should().Be("foo");
			p.PublishedFrom.Should().NotBeNull();
		});
	}

	[Fact]
	public void ItCanOverwriteProp()
	{
		var model = new AuthorFactory()
			.Property(a => a.Name, () => "bar")
			.Create();

		model.Name.Should().Be("bar");
	}

	[Fact]
	public void ItCanOverwriteMultipleProps()
	{
		Guid id = Guid.NewGuid();

		var model = new AuthorFactory()
			.Property(a => a.Id, () => id)
			.Property(a => a.Name, () => "baz")
			.Create();

		model.Id.Should().Be(id);
		model.Name.Should().Be("baz");
	}

	[Fact]
	public void ItCanOverwritePropertyUsingModel()
	{
		var model = new AuthorFactory()
			.Property(a => a.Name, (model) => model.Name + "bar")
			.Create();

		model.Name.Should().Be("foobar");
	}

	[Fact]
	public void GeneratingForNonWritablePropertyThrowsException()
	{
		Assert.Throws<PropIsReadOnlyException>(() =>
		{
			new AuthorFactory()
				.Property(a => a.NotWritable, () => "foo")
				.Create();
		});
	}

	[Fact]
	public void ItCanUsePredefinedState()
	{
		var model = new PostFactory().Published().Create();

		model.PublishedFrom.Should().NotBeNull();
	}

	[Fact]
	public void ItCanCombineStateWithOverrides()
	{
		var model = new PostFactory()
			.Published()
			.Property(p => p.Title, () => "::title::")
			.Create();

		model.PublishedFrom.Should().NotBeNull();
		model.Title.Should().Be("::title::");
	}

	[Fact]
	public void ItCanCombineMultipleStates()
	{
		var model = new PostFactory()
			.Published()
			.WithFooTitle()
			.Create();

		model.PublishedFrom.Should().NotBeNull();
		model.Title.Should().Be("foo");
	}

	[Fact]
	public void ItCanCreateModelWithRawValue()
	{
		var model = new PostFactory()
			.Property(p => p.Title, "::title::")
			.Create();

		model.Title.Should().Be("::title::");
	}
}
