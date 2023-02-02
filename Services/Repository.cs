using TestFecthOptimizer.Types;

namespace TestFecthOptimizer.Services;

public class Repository : IRepository
{
    public Repository()
    {
        Authors = new List<Author>() {
            new Author(1, "Robert Lafont"),
            new Author(2, "James Endryx"),
            new Author(3, "Rodolf Paulmann")
        }.AsQueryable();

        Books = new List<Book>() {
            new Book(1, "The book", 2),
            new Book(2, "The real book", 1),
            new Book(3, "The other book", 1),
            new Book(4, "The real super book", 3),
        }.AsQueryable();
    }

    public IQueryable<Author> Authors { get; }
    public IQueryable<Book> Books { get; }
}