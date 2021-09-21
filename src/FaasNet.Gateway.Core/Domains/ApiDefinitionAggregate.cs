using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionAggregate : ICloneable
    {
        public ApiDefinitionAggregate()
        {
            Operations = new List<ApiDefinitionOperation>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public ICollection<ApiDefinitionOperation> Operations { get; set; }

        #region Operations

        public void UpdatePath(string path)
        {
            Path = path;
        }

        public ApiDefinitionOperation UpdateOperation(string name, string path)
        {
            var op = Operations.FirstOrDefault(op => op.Name == name);
            if (op == null)
            {
                op = ApiDefinitionOperation.Create(name);
                Operations.Add(op);
            }

            op.UpdatePath(path);
            return op;
        }

        #endregion

        #region Getters

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

        #endregion

        public static ApiDefinitionAggregate Create(string name)
        {
            return new ApiDefinitionAggregate
            {
                Name = name
            };
        }

        public object Clone()
        {
            return new ApiDefinitionAggregate
            {
                Name = Name,
                Path = Path,
                Operations = Operations.Select(op => (ApiDefinitionOperation)op.Clone()).ToList()
            };
        }
    }
}
