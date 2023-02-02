using HotChocolate.Types.Descriptors;
using System.Reflection;
using TestFecthOptimizer.Services;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(Author))]
public static class AuthorType
{
    [DataLoader]
    internal static Task<IReadOnlyDictionary<int, Author>> GetAuthorByIdAsync(
    IReadOnlyList<int> keys,
    IRepository repository,
    CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyDictionary<int, Author>>(repository.Authors
            .Where(a => keys.Contains(a.Id))
            .ToDictionary(t => t.Id));
    }

    public static string? NameUpperCase([Parent] Author author) => author.Name?.ToUpperInvariant();

    [UseOnlyParentId]
    public static int IdDouble([Parent] Author author) => author.Id * 2;
}

public sealed class UseOnlyParentIdAttribute : ObjectFieldDescriptorAttribute
{
    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Extend().Definition.CustomSettings.Add(new UseOnlyParentId());
    }
}

public class UseOnlyParentId
{

}