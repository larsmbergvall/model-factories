# 1.1.1

## Bugfixes

* The internal Factory map now uses a concurrent dictionary to avoid issues when running in certain applications

# 1.1.0

## Features

### Simpler With-syntax

Simplified the API for creating related models, assuming you are using Factory Discovery or have mapped your factories
already. If you have, you can now call `new PostFactory().With<Post>(p => p.Post)`. In other words, you no longer need
to supply a generic argument for the factory type.

# 1.0.1

Target framework is now dotnet 6.0

# 1.0.0

No new features, mostly just making sure it works in dotnet 8 and bumping the version as it seems rather stable now

# 0.4.0

## Features

### Recycling models

Added the `Recycle(object)` method, which makes ModelFactories use a given instance of a model instead of creating a new
one. See readme for more details.

# 0.3.0

## Features

### WithMany:

If your model has a List of related models (for example a Blog can have many Post objects), these can now be generated
in the same way you can generate nested models with `With`.
Example: `new BlogFactory().WithMany<Post, PostFactory>(b => b.Posts, 5)`

See README.md for more info and examples

# 0.2.5

## Bugs

Fixed a crash that could occur when mapping (either automatically or manually) to add the same factory twice. For
example, if you run the auto discovery before each unit test, it would crash.

# 0.2.4

## Features

Implemented Factory Discovery:

Added a FactoryMap class which can either manually or automatically discover
factories for classes, so you can call `FactoryMap.FactoryFor<Post>()`,
or implement something on your base model class.
See [README.md](README.md) for more details.

# 0.2.3

## Bugs

Fixed a bug where using batch creation (Create(int) or CreateMany(int))
didn't generate new values for each created model

# 0.2.2

## Bugs

Fixed a bug where calling .Property() sometimes would not override the property.

For example, if your Definition() method had a property using a model callback:

```csharp
// inside Definition() method:
Property(post => post.PublishedFrom, (post) => post.CreatedAt);

// Somewhere else, maybe in your test:
new PostFactory().Property(post => post.PublishedFrom, () => null);
```

the result would always be `post.CreatedAt`. 
