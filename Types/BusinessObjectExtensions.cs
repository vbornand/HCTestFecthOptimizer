using TestFecthOptimizer.Infrastructure;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(IBusinessObject))]
public static class BusinessObjectExtensions
{
    [UseOnlyParentId]
    public static DateTime Now([Parent] IBusinessObject _) => DateTime.Now;
}
