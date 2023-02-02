This small HotChocolate project includes a feature that eliminates the need to call data loaders when all requested fields rely solely on the ID field for resolution using resolvers.

This query does not execute the `IAuthorByIdDataLoader` because the fields `id` and `idDouble` resolvers use only the author iD.
```
{
  books {
    edges
    {
      node {
        id
        title
        author {
          id
          idDouble
        }
      }
    }
  }
}
```

This query executes the `IAuthorByIdDataLoader` because the field `nameUpperCase` resolvers need to have a "full" author with the name.
```
{
  books {
    edges
    {
      node {
        id
        title
        author {
          id
          nameUpperCase
        }
      }
    }
  }
}
```

Currently there is a custom `OptimizedFetchAttribute` for each type with an hardcoded list of fields bypassing the data loader.

A better implementation will use a generic attribute which scans the requested type, to check which fields(resolvers) can be
executed with only the id. With per exemple a `UseOnlyParentId` attribute.

The optimized resolver is `GetAuthor` in `Types\BookType.cs` file.