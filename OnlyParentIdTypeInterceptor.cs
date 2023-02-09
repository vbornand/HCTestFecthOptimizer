using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;
using System.Reflection;
using TestFecthOptimizer.Infrastructure;
using TestFecthOptimizer.Services;

internal class OnlyParentIdTypeInterceptor : TypeInterceptor
{
    private readonly ITypesOnlyParentIdService typesOnlyParentIdService;

    public OnlyParentIdTypeInterceptor(ITypesOnlyParentIdService typesOnlyParentIdService)
    {
        this.typesOnlyParentIdService = typesOnlyParentIdService;
    }

    public override void OnAfterCreateSchema(IDescriptorContext context, ISchema schema)
    {
        base.OnAfterCreateSchema(context, schema);
        foreach (var typeName in schema.Types.Where(t => t.IsObjectType()))
        {
            var type = schema.GetType<IObjectType>(typeName.Name);
            if (type != null)
            {
                foreach (var field in type.Fields)
                {
                    if (field.ResolverMember is MethodBase mb)
                    {
                        if (mb.GetParameters().Any(p => p.GetCustomAttribute<ParentIdAttribute>() != null) &&
                            mb.GetParameters().Any(p => p.GetCustomAttribute<ParentAttribute>() == null))
                        {
                            typesOnlyParentIdService.RegisterFieldAsUseOnlyParentId(type.Name, field.Name);
                        }
                    }
                }
                typesOnlyParentIdService.RegisterFieldAsUseOnlyParentId(type.Name, "__typename");
            }
        }
    }
}