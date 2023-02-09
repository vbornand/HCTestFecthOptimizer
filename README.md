This small HotChocolate project includes a feature that eliminates the need to call data loaders when all requested fields rely solely on the ID field for resolution using resolvers.

This query does not execute the `IAuthorByIdDataLoader` because the fields `id`, `now` and `idDouble` resolvers use only the author iD.
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
          now
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

The type interceptor `OnlyParentIdTypeInterceptor` scans all the types, and adds in a list all the fields having a resolvers with a parameter with a `[ParentId]` attribute.

Example of field resolver using the ParentId: (The value is available even if the parent has been fully loaded.)
```
public static int IdDouble([ParentId]int parentId) => parentId * 2;
```

For the resolvers providing the parents, the attribute `SkipResolverIfOnlyIdRequired` can be added to avoid calling it if no 
fields requires the entire Parent object.
This attribute works only if the attribute `BindMember` is used in the same time.

```
[BindMember(nameof(Book.AuthorId))]
[SkipResolverIfOnlyIdRequired]
public static async Task<Author> GetAuthor([Parent] Book book,
                                            IAuthorByIdDataLoader authorByIdDataLoader,
                                            CancellationToken cancellationToken)
{
    return await authorByIdDataLoader.LoadAsync(book.AuthorId, cancellationToken);
}
```

The current implementation doesn't support 2 schemas using the same type names.