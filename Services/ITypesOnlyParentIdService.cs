namespace TestFecthOptimizer.Services
{
    public interface ITypesOnlyParentIdService
    {
        IReadOnlyList<string> GetUseOnlyParentIdFields(string typeName);
        void RegisterFieldAsUseOnlyParentId(string typeName, string fieldName);
    }
}