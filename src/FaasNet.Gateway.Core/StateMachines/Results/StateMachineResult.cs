using FaasNet.Runtime.Domains.Definitions;
using System;

namespace FaasNet.Gateway.Core.StateMachines.Results
{
    public class StateMachineResult
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }


        public static StateMachineResult ToDto(WorkflowDefinitionAggregate workflowDefinition)
        {
            return new StateMachineResult
            {
                Id = workflowDefinition.Id,
                CreateDateTime = workflowDefinition.CreateDateTime,
                Description = workflowDefinition.Description,
                Name = workflowDefinition.Name,
                UpdateDateTime = workflowDefinition.UpdateDateTime,
                Version = workflowDefinition.Version
            };
        }
    }
}
