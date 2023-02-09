using TestFecthOptimizer.Infrastructure;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(Book))]
public static class BookExtensions
{
    [BindMember(nameof(Book.AuthorId))]
    [SkipResolverIfOnlyIdRequired]
    public static async Task<Author> GetAuthor([Parent]Book book,
                                               IAuthorByIdDataLoader authorByIdDataLoader,
                                               CancellationToken cancellationToken)
    {
        Console.WriteLine("Author fetched.");
        return await authorByIdDataLoader.LoadAsync(book.AuthorId, cancellationToken);
    }

    private static int GetAuthorId(Book book)
    {
        Console.WriteLine("Author not fetched.");
        return book.AuthorId;
    }
}
