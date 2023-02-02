using TestFecthOptimizer.Services;

namespace TestFecthOptimizer.Types;

[QueryType]
public static class Query
{
    [UsePaging]
    public static IQueryable<Book> GetBooks([Service]IRepository repository)
        => repository.Books;
}
