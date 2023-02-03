using System.Reflection;
using HotChocolate.Data.Projections.Context;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using TestFecthOptimizer.Services;
using static System.Reflection.BindingFlags;

namespace TestFecthOptimizer.Infrastructure;

public sealed class SkipResolverIfOnlyIdRequiredAttribute : ObjectFieldDescriptorAttribute
{
    private MethodInfo? _getObjectWithOnlyIdMethod;
    private ITypesOnlyParentIdService? _typesOnlyParentIdService;
    public HashSet<string> _fieldsAvailableWithoutFetch = new HashSet<string>(); //TODO: Find the best type for performance

    private string? typeName;
    private string? fieldName;


    public string With { get; }

    public SkipResolverIfOnlyIdRequiredAttribute(string with)
    {
        With = with;
    }

    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        _typesOnlyParentIdService = context.Services.GetRequiredService<ITypesOnlyParentIdService>();

        descriptor.Extend().OnBeforeNaming((c, d) =>
        {
            _getObjectWithOnlyIdMethod = member.DeclaringType?.GetMethod(
                                      With,
                                      Public | NonPublic | Static);
        });

        descriptor.Extend().OnBeforeCompletion((c, d) =>
        {
            //Save the type name and field name, to get the field in the schema when it is completed to retrieve the GraphQL type returned
            //by the resolver.
            typeName = c.ToString()!; //Todo: Better way to get the type name?
            fieldName = d.Name;
        });

        //It is not possible to use the OnBeforeCompletion().DependsOn() because we don't know yet the result type name.

        context.SchemaCompleted += (_, e) =>
        {
            if (!string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(fieldName))
            {
                var t = e.Schema.GetType<IObjectType>(typeName);
                if (t.Fields.TryGetField(fieldName, out var f))
                {
                    var typeName = f.Type.NamedType();
                    _fieldsAvailableWithoutFetch = _typesOnlyParentIdService!.GetUseOnlyParentIdFields(typeName.Name).ToHashSet();
                    _fieldsAvailableWithoutFetch.Add("__typename");
                }
            }
        };

        descriptor.Use(next => async context =>
        {
            if (!context.IsResultModified && _getObjectWithOnlyIdMethod != null && GetFetchStrategy(context) == FetchStrategy.OnlyId)
            {
                context.Result = _getObjectWithOnlyIdMethod.Invoke(null, new object[] { context.Parent<object>() });
            }
            await next(context);
        });
    }

    private FetchStrategy GetFetchStrategy(IResolverContext context)
    {
        //TODO: Find a way to avoid to use HotChocolate.Data.Projections.
        var fields = context.GetSelectedField().GetFields().Select(f => f.Field.Name);


        foreach (var field in fields)
        {
            if (!_fieldsAvailableWithoutFetch.Contains(field))
            {
                return FetchStrategy.All;
            }
        }

        return FetchStrategy.OnlyId;
    }
}
