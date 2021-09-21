using FaasNet.Gateway.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperation : ICloneable
    {
        public ApiDefinitionOperation()
        {
            SequenceFlows = new List<ApiDefinitionSequenceFlow>();
            Functions = new List<ApiDefinitionFunction>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public ICollection<ApiDefinitionSequenceFlow> SequenceFlows { get; set; }
        public ICollection<ApiDefinitionFunction> Functions { get; set; }

        #region Operations

        public void UpdatePath(string path)
        {
            Path = path;
        }

        public void UpdateFunction(string function, string configuration)
        {
            var fn = Functions.FirstOrDefault(fn => fn.Function == function);
            if (fn == null)
            {
                fn = ApiDefinitionFunction.Create(function);
                Functions.Add(fn);
            }

            fn.UpdateConfiguration(configuration);
        }

        public void UpdateSequenceFlow(string sourceRef, string targetRef)
        {
            var sequenceFlow = SequenceFlows.FirstOrDefault(fl => fl.SourceRef == sourceRef && fl.TargetRef == targetRef);
            if (sequenceFlow == null)
            {
                SequenceFlows.Add(ApiDefinitionSequenceFlow.Create(sourceRef, targetRef));
            }
        }

        #endregion

        #region Getters

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

        #endregion

        public static ApiDefinitionOperation Create(string name)
        {
            return new ApiDefinitionOperation
            {
                Name = name
            };
        }

        public object Clone()
        {
            return new ApiDefinitionOperation
            {
                Name = Name,
                Path = Path,
                SequenceFlows = SequenceFlows.Select(sf => (ApiDefinitionSequenceFlow)sf.Clone()).ToList(),
                Functions = Functions.Select(fn => (ApiDefinitionFunction)fn.Clone()).ToList()
            };
        }
    }
}
