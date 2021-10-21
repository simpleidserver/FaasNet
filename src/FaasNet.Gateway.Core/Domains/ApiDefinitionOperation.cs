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
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ApiDefinitionOperationUI UI { get; set; }
        public ICollection<ApiDefinitionSequenceFlow> SequenceFlows { get; set; }
        public ICollection<ApiDefinitionFunction> Functions { get; set; }

        #region Operations

        public void UpdatePath(string path)
        {
            Path = path;
        }

        public void AddFunction(string id, string name, string configuration)
        {
            var fn = ApiDefinitionFunction.Create(id, name);
            fn.UpdateConfiguration(configuration);
            Functions.Add(fn);
        }

        public void AddSequenceFlow(string sourceRef, string targetRef)
        {
            SequenceFlows.Add(ApiDefinitionSequenceFlow.Create(sourceRef, targetRef));
        }

        public void UpdateUI(ApiDefinitionOperationUI ui)
        {
            UI = ui;
            Functions.Clear();
            SequenceFlows.Clear();
            UpdateDateTime = DateTime.UtcNow;
            foreach(var block in ui.Blocks)
            {
                if (block.Model == null || block.Model.Info == null)
                {
                    continue;
                }

                var newFn = ApiDefinitionFunction.Create(block.ExternalId.ToString(), block.Model.Info.Name);
                newFn.SerializedConfiguration = block.Model.Configuration?.ToString();
                Functions.Add(newFn);
                if (block.Parent != -1)
                {
                    SequenceFlows.Add(ApiDefinitionSequenceFlow.Create(block.Parent.ToString(), block.ExternalId.ToString()));
                }
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
            return Functions.FirstOrDefault(s => !SequenceFlows.Any(f => f.TargetRef == s.Name));
        }

        public ApiDefinitionFunction GetNextFunction(ApiDefinitionFunction fn)
        {
            var sequenceFlow = SequenceFlows.FirstOrDefault(f => f.SourceRef == fn.Name);
            if (sequenceFlow == null)
            {
                return null;
            }

            return Functions.FirstOrDefault(f => f.Name == sequenceFlow.TargetRef);
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
                Functions = Functions.Select(fn => (ApiDefinitionFunction)fn.Clone()).ToList(),
                UI = (ApiDefinitionOperationUI)UI?.Clone(),
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime
            };
        }
    }
}
