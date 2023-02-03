using TestFecthOptimizer.Infrastructure;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(Book))]
public static class BookExtensions
{
    [BindMember(nameof(Book.AuthorId))]
    [SkipResolverIfOnlyIdRequired(nameof(GetAuthorWithOnlyId))]
    public static async Task<Author> GetAuthor([Parent] Book book,
                                               IAuthorByIdDataLoader authorByIdDataLoader,
                                               CancellationToken cancellationToken)
    {
        Console.WriteLine("Author fetched.");
        return await authorByIdDataLoader.LoadAsync(book.AuthorId, cancellationToken);
    }

    private static Author GetAuthorWithOnlyId(Book book)
    {
        Console.WriteLine("Author not fetched.");
        return new Author(book.AuthorId, null);
    }
}
