using FaasNet.Gateway.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperation
    {
        public ApiDefinitionOperation()
        {
            SequenceFlows = new List<ApiDefinitionSequenceFlow>();
            Functions = new List<ApiDefinitionFunction>();
        }

        public string Path { get; set; }
        public ICollection<ApiDefinitionSequenceFlow> SequenceFlows { get; set; }
        public ICollection<ApiDefinitionFunction> Functions { get; set; } 

        public bool TryMatch(string subPath, out Dictionary<string, string> parameters)
        {
            parameters = null;
            subPath = subPath.CleanPath();
            var subPathValues = subPath.Split('/');
            var pathParameters = subPath.Split('/').Select(s => s.Replace("{", string.Empty).Replace("}", string.Empty));
            if (pathParameters.Count() != subPathValues.Count())
            {
                return false;
            }

            parameters = new Dictionary<string, string>();
            for(int i = 0; i < pathParameters.Count(); i++)
            {
                parameters.Add(pathParameters.ElementAt(i), subPathValues[i]);
            }

            return true;
        }

        public ApiDefinitionFunction GetRootFunction()
        {
            return Functions.FirstOrDefault(s => !SequenceFlows.Any(f => f.TargetRef == s.Function));
        }

        public ApiDefinitionFunction GetNextFunction(ApiDefinitionFunction fn)
        {
            var sequenceFlow = SequenceFlows.FirstOrDefault(f => f.SourceRef == fn.Function);
            if (sequenceFlow == null)
            {
                return null;
            }

            return Functions.FirstOrDefault(f => f.Function == sequenceFlow.TargetRef);
        }
    }
}
