using System.Reflection;
using HotChocolate.Data.Projections.Context;
using HotChocolate.Internal;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(Book))]
public static class BookType
{
    [BindMember(nameof(Book.AuthorId))]
    [AuthorOptimizedFetch]
    public static async Task<Author> GetAuthor([Parent] Book book,
                                               IAuthorByIdDataLoader authorByIdDataLoader,
                                               FetchStrategy fetchStrategy,
                                               CancellationToken cancellationToken)
    {
        if (fetchStrategy == FetchStrategy.OnlyId)
        {
            Console.WriteLine("Author not fetched.");
            return new Author(book.AuthorId, null);
        }
        else
        {
            Console.WriteLine("Author fetched.");
            return await authorByIdDataLoader.LoadAsync(book.AuthorId, cancellationToken);
        }
    }
}

public sealed class AuthorOptimizedFetchAttribute : OptimizedFetchAttribute
{
    public AuthorOptimizedFetchAttribute(): base(new string[] { "__typename", "id", "idDouble" })
    {
    }
}

public abstract class OptimizedFetchAttribute : ObjectFieldDescriptorAttribute
{
    public string[] FieldsAvailableWithoutFetch { get; }

    public OptimizedFetchAttribute(string[] fieldsAvailableWithoutFetch)
    {
        this.FieldsAvailableWithoutFetch = fieldsAvailableWithoutFetch;
    }

    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Extend().Definition.ParameterExpressionBuilders.Add(
            new CustomParameterExpressionBuilder<FetchStrategy>(ctx => GetFetchStrategy(ctx))
        );
    }

    private FetchStrategy GetFetchStrategy(IResolverContext context)
    {
        var fields = context.GetSelectedField().GetFields().Select(f => f.Field.Name);

        if ((FieldsAvailableWithoutFetch == null) || fields.Except(FieldsAvailableWithoutFetch).Any())
        {
            return FetchStrategy.All;
        }
        else
        {
            return FetchStrategy.OnlyId;
        }
    }
}

public enum FetchStrategy
{
    OnlyId,
    All
}
