using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionAggregate : ICloneable
    {
        private static ICollection<string> STANDARD_URLS = new List<string>
        {
            "apis", "configuration", "functions"
        };

        public ApiDefinitionAggregate()
        {
            Operations = new List<ApiDefinitionOperation>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<ApiDefinitionOperation> Operations { get; set; }

        #region Operations

        public void UpdatePath(string path)
        {
            Path = path;
        }

        public ApiDefinitionOperation AddOperation(string name, string path)
        {
            if (Operations.Any(o => o.Name == name))
            {
                throw new BusinessRuleException(string.Format(Global.OperationExists, name));
            }

            var op = ApiDefinitionOperation.Create(name);
            op.UpdatePath(path);
            Operations.Add(op);
            return op;
        }

        public void UpdateUIOperation(string opName, ApiDefinitionOperationUI ui)
        {
            var op = GetOperation(opName);
            if (op == null)
            {
                throw new BusinessRuleException(string.Format(Global.UnknownApiDefOperation, opName));
            }

            op.UpdateUI(ui);
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

        public ApiDefinitionOperation GetOperation(string name)
        {
            return Operations.FirstOrDefault(o => o.Name == name);
        }

        #endregion

        public static ApiDefinitionAggregate Create(string name, string path = null)
        {
            if (STANDARD_URLS.Contains(path))
            {
                throw new BusinessRuleException(string.Format(Global.StandardPath, path));
            }

            return new ApiDefinitionAggregate
            {
                Name = name,
                Path = path,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }

        public object Clone()
        {
            return new ApiDefinitionAggregate
            {
                Name = Name,
                Path = Path,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Operations = Operations.Select(op => (ApiDefinitionOperation)op.Clone()).ToList()
            };
        }
    }
}
