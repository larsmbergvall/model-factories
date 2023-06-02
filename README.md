# C# Model Factories

Very simple package for creating class based model factories. Inspired by
Laravel [Model Factories](https://laravel.com/docs/eloquent-factories)

It uses the [Bogus library](https://github.com/bchavez/Bogus) under the hood.

## Usage

To use Model Factories, you create a class for your factory, i.e. `PostFactory.cs`

```csharp
public class PostFactory : ModelFactory<Post>
{
    protected override void Definition()
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Title, f => string.Join(' ', f.Lorem.Words(5)));
        RuleFor(x => x.Body, f => string.Join('\n', f.Lorem.Paragraphs(
            f.Random.UShort(3, 10))
        ));
        RuleFor(
            x => x.PublishedFrom,
            f => f.Date.Between(DateTime.Today.AddYears(-2), DateTime.Today)
        );
    }
}
```

Then you can simply create Post objects by using:

```csharp
// Create a single post
Post post = new PostFactory().Create();

// Create many posts
List<Post> posts = new PostFactory().Create(2);
```

### Overriding attributes when creating

When creating, if you want a specific property value on your generated object(s),
you can send them as method arguments using the same signature as Bogus' `RuleFor` as Tuples:

```csharp
// One
Post post = new PostFactory().Create(
    (p => p.Title, f => "New title"),
    (p => p.Body, f => "New Body"),
);

// Many; all created posts will have null as their PublishedFrom value
List<Post> posts = new PostFactory().Create(2,
    (p => p.PublishedFrom, f => null)
);
```

### Factory states

You might want to reuse the code for creating posts with PublishedFrom = null, without having to type that again. This
is where factory states come into play.

In the model factory, you can define a custom method for any state you want. This is basically the same as overriding an
attribute when creating:

```csharp
public PostFactory Draft()
{
    return (PostFactory) State(
        (x => x.PublishedFrom, f => null)
    );
}
```

You can send multiple attribute overrides to the State method. You can also combine multiple states when creating your
models:

```csharp
var post = new PostFactory()
    .Draft()
    .WithTitle("Foo")
    .Create();
```