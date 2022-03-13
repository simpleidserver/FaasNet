using FaasNet.StateMachine.Runtime.Domains.Definitions;
using System;

namespace FaasNet.Gateway.Core.StateMachines.Results
{
    public class StateMachineResult
    {
        public string TechnicalId { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }


        public static StateMachineResult ToDto(WorkflowDefinitionAggregate workflowDefinition)
        {
            return new StateMachineResult
            {
                TechnicalId = workflowDefinition.TechnicalId,
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
