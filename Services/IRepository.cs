using TestFecthOptimizer.Types;

namespace TestFecthOptimizer.Services;

public interface IRepository
{
    IQueryable<Author> Authors { get; }
    IQueryable<Book> Books { get; }
}
