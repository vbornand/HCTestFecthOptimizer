namespace TestFecthOptimizer.Services
{
    public class TypesOnlyParentIdService : ITypesOnlyParentIdService
    {
        private Dictionary<string, List<string>> _typesFieldsDictionary;

        public TypesOnlyParentIdService()
        {
            this._typesFieldsDictionary = new Dictionary<string, List<string>>();
        }

        public void RegisterFieldAsUseOnlyParentId(string typeName, string fieldName)
        {
            if (this._typesFieldsDictionary.TryGetValue(typeName, out var fields))
            {
                fields.Add(fieldName);
            }
            else
            {
                this._typesFieldsDictionary.Add(typeName, new List<string>() { fieldName });
            }
        }

        public IReadOnlyList<string> GetUseOnlyParentIdFields(string typeName)
        {
            if (this._typesFieldsDictionary.TryGetValue(typeName, out var fields))
            {
                return fields;
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
