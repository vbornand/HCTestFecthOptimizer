namespace TestFecthOptimizer.Infrastructure
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParentIdAttribute : ScopedStateAttribute
    {
        public ParentIdAttribute() : base("ParentId")
        {
        }
    }
}
