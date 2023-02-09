using TestFecthOptimizer.Infrastructure;

namespace TestFecthOptimizer.Types;

[ExtendObjectType(typeof(IBusinessObject))]
public static class BusinessObjectExtensions
{
    public static DateTime Now([ParentId]object _) => DateTime.Now;
}
