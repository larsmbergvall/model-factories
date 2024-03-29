# C# Model Factories

Very simple package for creating class based model factories. Inspired by
Laravel [Model Factories](https://laravel.com/docs/eloquent-factories)

The intended use is in tests and when you need to seed data for local development.

**[Changelog](https://github.com/larsmbergvall/model-factories/blob/main/CHANGELOG.md)**

## Installing

```
dotnet add package ModelFactories
```

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
            // You can also use raw values instead of callbacks:
            //.Property(p => p.Title, "Hardcoded title")
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

It is possible to use factories to generate related models for a model as well. For instance, a Post might have an
Author.
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

For List properties, use `WithMany`:

```csharp
// Create 5 random authors
WithMany<Author, AuthorFactory>(p => p.Authors, 5);

// Also create 5 random authors but mutating the factory. Callback must return a List of items
WithMany<Author, AuthorFactory>(
    p => p.Authors,
    factory => factory.Property(a => a.Name, "John Doe").CreateMany(5)
);

// Create 5 random authors, but mutate the factory state. If using this approach, a new factory will be created for each item.
// This means you can create things in sequence. 
// In this example, each created Author will have a Score, where the first one has a score of 1 and the last one 5
int score = 0;

WithMany<Author, AuthorFactory>(
    p => p.Authors,
    5,
    factory => factory.Property(
        a => c.Score,
        () =>
        {
            score++;

            return score;
        }
    )
);
```

There is also a simpler syntax for creating related/nested models, which is described in the section
[Factory Discovery](#factory-discovery). You need to use Factory Discovery/mapping to use it.

### Hooks

If you need to manipulate the created model after it was created, you can add
any number of callbacks to be executed:

```csharp
var post = new PostFactory()
	.AfterCreate(post =>
		{
			post.AuthorId = post.Author.Id;
			return post;
		})
	.Create();
```

Of course, this can also be done inside your ModelFactory `Definition()` method.

### Factory Discovery

Factory Discovery is an **optional (but recommended) feature**, which allows you to get a factory for a generic type,
like this:

```csharp
var postFactory = FactoryMap.FactoryFor<Post>();
```

This might be useful in case you have a base class for your models, and want a handy static method for
getting a factory for a given model. For example:

```csharp
var postFactory = Post.Factory();
```

One way of achieving this is by having the base model take a generic type of the model itself:

```csharp
// BaseModel.cs
using ModelFactories;

public abstract class BaseModel<T> where T: class, new()
{
    public static ModelFactory<T> Factory()
    {
        return FactoryMap.FactoryFor<T>();
    }
}

// Post.cs
public class Post : BaseModel<Post>
{
    // ...
}
```

Of course, you could also have a non-generic BaseModel and simply return a `ModelFactory<BaseModel>`.

**One limitation** of using factory discovery is that the returned type is `ModelFactory<T>`, which means
that you don't have access to any custom states or methods defined in your factory, unless you cast it:

```csharp
var postFactory = (PostFactory) Post.Factory();
```

#### Simpler With() calls

If you are using factory discovery, regardless of if you are mapping them automatically or manually, you can use
a simpler With-syntax when creating related/nested models:

```csharp
var post = new PostFactory()
    .With<Author>(p => p.Author)
    // or .With<Author>(p => p.Author, factory => factory.Property(a => a.Name, "John Doe").Create())
    .WithMany<Comment>(p => p.Comments, 10)
    // or .WithMany<Comment>(p => p.Comments, 10, factory => factory.Property(c => c.Title, "Foo"))
    // or .WithMany<Comment>(p => p.Comments, factory => factory.Property(c => c.Title, "Foo").CreateMany(10))
    .Create();
```

#### How are factories discovered?

There are two ways of mapping types to factories: Automatic Discovery and manual mapping.

Factories can be automatically discovered by calling:

```csharp
// Note: Assembly must be the assembly containing the ModelFactory classes!
FactoryMap.DiscoverFactoriesInAssembly(Assembly.GetExecutingAssembly());
```

Factories can be manually mapped using:

```csharp
FactoryMap.Map<Comment, CommentFactory>();
```

### Reusing model when creating nested models (recycling)

If you want to reuse a certain instance of a model when nested objects are being generated, you can use the `Recycle()`
method:

```csharp
var someAuthor = new AuthorFactory().Create();
var blog = new BlogFactory()
    .WithMany<Post, PostFactory>(b => b.Posts, 10)
    .Recycle(someAuthor)
    .Create();
```

In this example, we create a `Blog` model which has 10 `Post` models. Each `Post` has an `Author`. When calling
`Recycle(someAuthor)`, all 10 posts will use `someAuthor` whenever a factory (including all nested factories) would
otherwise generate a new `Author` object.

## Combining with Bogus/Faker

If you want to use Bogus for generating data, you can create an extension for ModelFactory in your project:

```csharp
public static class ModelFactoryExtensions
{
    public static Faker CreateFaker<T>(this ModelFactory<T> modelFactory)
        where T : class, new()
    {
        return new Faker("en");
    }
}
```

With this extension in place you can call CreateFaker in your factories:

```csharp
public class PostFactory : ModelFactory<Post>
{
    protected override void Definition()
    {
        var faker = this.CreateFaker();

        Property(p => p.Id, () => faker.Random.Guid());
    }
}
```
