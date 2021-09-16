using System.Collections.Generic;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionAggregate
    {
        public ApiDefinitionAggregate()
        {
            Operations = new List<ApiDefinitionOperation>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public ICollection<ApiDefinitionOperation> Operations { get; set; }

        public bool TryGetOperation(string fullPath, out ApiDefinitionOperation operation, out Dictionary<string, string> parameters)
        {
            operation = null;
            parameters = null;
            var subPath = fullPath.Replace(Path, string.Empty);
            foreach(var op in Operations)
            {
                if (op.TryMatch(subPath, out parameters))
                {
                    operation = op;
                    return true;
                }
            }

            return false;
        }
    }
}
