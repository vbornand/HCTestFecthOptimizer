using System.Reflection;
using HotChocolate.Data.Projections.Context;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using TestFecthOptimizer.Services;

namespace TestFecthOptimizer.Infrastructure;

public sealed class SkipResolverIfOnlyIdRequiredAttribute : ObjectFieldDescriptorAttribute
{
    private ITypesOnlyParentIdService? _typesOnlyParentIdService;
    private PropertyInfo? bindedProperty;

    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        _typesOnlyParentIdService = context.Services.GetRequiredService<ITypesOnlyParentIdService>();

        descriptor.Extend().OnBeforeNaming((c, d) =>
        {
            if (d.BindToField.HasValue &&
                //TODO: Add support for Type = Field.
                d.BindToField.Value.Type == HotChocolate.Types.Descriptors.Definitions.ObjectFieldBindingType.Property)
            {
                var propertyName = d.BindToField.Value.Name;
                if (member is MethodBase mb)
                {
                    var parentParameter = mb.GetParameters().FirstOrDefault(p => p.GetCustomAttribute<ParentAttribute>() != null);
                    if (parentParameter != null)
                    {
                        bindedProperty = parentParameter.ParameterType.GetProperty(propertyName);
                    }
                }
                if (bindedProperty == null)
                {
                    //TODO: Add details in the error
                    c.ReportError(SchemaErrorBuilder.New().SetMessage("Unable to get the id provider").Build());
                }
            }
        });

        descriptor.Use(next => async context =>
        {
            var parent = context.Parent<object>();
            if (parent != null)
            {
                var parentId = bindedProperty!.GetValue(parent);
                if (parentId != null)
                {
                    context.SetScopedState("ParentId", parentId);
                    if (!context.IsResultModified && GetFetchStrategy(context, _typesOnlyParentIdService) == FetchStrategy.OnlyId)
                    {
                        //It's better to call the other middlewares, but to avoid to execute
                        //the resolver, we need to set the result with a fake value.
                        context.Result = UnresolvedParent.Instance;
                    }
                }
            }
            await next(context);
        });
    }

    private FetchStrategy GetFetchStrategy(IResolverContext context, ITypesOnlyParentIdService typesOnlyParentIdService)
    {
        var selectedField = context.GetSelectedField();
        var typeName = selectedField.Type.TypeName();

        //TODO: Find a way to avoid to use HotChocolate.Data.Projections.
        var fields = selectedField.GetFields().Select(f => f.Field.Name);

        var _fieldsAvailableWithoutFetch = typesOnlyParentIdService.GetUseOnlyParentIdFields(typeName);

        foreach (var field in fields)
        {
            if (!_fieldsAvailableWithoutFetch.Contains(field))
            {
                return FetchStrategy.All;
            }
        }

        return FetchStrategy.OnlyId;
    }

    internal class UnresolvedParent
    {
        public static UnresolvedParent Instance { get; set; } = new UnresolvedParent();
    }
}
