using HotChocolate.Types.Descriptors;
using System.Reflection;

namespace TestFecthOptimizer.Infrastructure;

public sealed class UseOnlyParentIdAttribute : ObjectFieldDescriptorAttribute
{
    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.UseOnlyParentId();
    }
}
