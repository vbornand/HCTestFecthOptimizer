using TestFecthOptimizer.Services;

namespace TestFecthOptimizer.Infrastructure;

public static class OnlyParentIdObjectFieldExtensions
{
    public static IObjectFieldDescriptor UseOnlyParentId(this IObjectFieldDescriptor descriptor)
    {
        descriptor.Extend().OnBeforeCompletion((completionContext, field) =>
        {
            var topis = completionContext.Services.GetRequiredService<ITypesOnlyParentIdService>();
            topis.RegisterFieldAsUseOnlyParentId(completionContext.Type.Name, field.Name);
        });

        return descriptor;
    }
}