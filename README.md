# C# Model Factories

Very simple package for creating class based model factories. Inspired by
Laravel [Model Factories](https://laravel.com/docs/eloquent-factories)

This package is still in early development, so expect bugs and breaking changes :)

## Usage

To use Model Factories, you create a class for your factory, i.e. `PostFactory.cs`

```csharp
public class PostFactory : ModelFactory<Post>
{
    protected override void Definition()
    {
        Property(p => p.Id, () => Guid.NewGuid())
            .Property(p => p.Title, () => "Post title")
            .Property(p => p.Body, () => "Lorem ipsum")
            .Property(p => p.CreatedAt, () => DateTime.Now);
    }
}
```

Then you can simply create Post objects by using:

```csharp
// Create a single post
Post post = new PostFactory().Create();

// Create many posts
List<Post> posts = new PostFactory().CreateMany(2);
```

### Overriding attributes when creating

When creating, if you want to override a specific property value on your generated object(s),
you can just call the `Property()` method on the factory:

```csharp
Post post = new PostFactory()
    .Property(p => p.Title, () => "New title")
    .Property(p => p.UpdatedAt, (model) => model.CreatedAt)
    .Create();
```

### Factory states

You might want to reuse the code for creating posts with PublishedFrom = null, without having to type that again. This
is where factory states come into play.

In the model factory, you can define a custom method for any state you want.

```csharp
public PostFactory Draft()
{
    Property(p => p.PublishedFrom, () => null);

    return this;
}
```

You can then use your states like this:

```csharp
var post = new PostFactory()
    .Draft()
    // You can also combine multiple states
    .WithTitle("Foo")
    .Create();
```

### Related/Nested models and factories

It is possible to use factories to generate related models for a model as well. For instance, a Post might have an Author.
In this case you can tell the ModelFactory to use the Author factory when creating that resource:

```csharp
protected override void Definition()
{
    // Other calls to Property, etc.

    // The generic types specify which model and factory to use
    // the function parameters specify which property on this factory model to use.
    With<Author, AuthorFactory>(post => post.Author);
    
    // You can also send a callback to modify the related factory. The callback must return the created model(s)
    With<Author, AuthorFactory>(p => p.Author, (authorFactory) => {
        return authorFactory
            .Property(author => author.Name, () => "Foo")
            .Create();
    });
}
```

Of course, this can also be called on a specific factory instance:

```csharp
    var post = new PostFactory()
        .With<Author, AuthorFactory>()
        .Create();
```
