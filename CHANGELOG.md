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
