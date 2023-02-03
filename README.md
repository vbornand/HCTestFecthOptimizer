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

It is possible to define which resolvers use only the ID of the parent with the attribute `UseOnlyParentIdAttribute` or with `descriptor.Field(a => a.Id).UseOnlyParentId();`

For the resolvers providing the parents, a second method generating only the object with ID is required:
```
[SkipResolverIfOnlyIdRequired(nameof(GetAuthorWithOnlyId))]
public static async Task<Author> GetAuthor([Parent] Book book,
                                            IAuthorByIdDataLoader authorByIdDataLoader,
                                            CancellationToken cancellationToken)
{
    return await authorByIdDataLoader.LoadAsync(book.AuthorId, cancellationToken);
}

//must be static
private static Author GetAuthorWithOnlyId(Book book)
{
    return new Author(book.AuthorId, null);
}
```

The current implementation doesn't support 2 schemas using the same type names.