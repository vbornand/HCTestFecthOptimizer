using TestFecthOptimizer.Infrastructure;
using TestFecthOptimizer.Services;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(Author))]
public static class AuthorExtensions
{
    [DataLoader]
    internal static Task<IReadOnlyDictionary<int, Author>> GetAuthorByIdAsync(
    IReadOnlyList<int> keys,
    IRepository repository,
    CancellationToken cancellationToken)
    {
        Console.WriteLine($"IAuthorByIdDataLoader - keys: {string.Join(", ", keys)}");

        return Task.FromResult<IReadOnlyDictionary<int, Author>>(repository.Authors
            .Where(a => keys.Contains(a.Id))
            .ToDictionary(t => t.Id));
    }

    public static string? NameUpperCase([Parent] Author author) => author.Name?.ToUpperInvariant();

    [UseOnlyParentId]
    public static int IdDouble([Parent] Author author) => author.Id * 2;
}
