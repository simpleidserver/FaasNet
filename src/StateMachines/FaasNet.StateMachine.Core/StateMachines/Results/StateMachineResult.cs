using FaasNet.StateMachine.Runtime.Domains.Definitions;
using System;

namespace FaasNet.StateMachine.Core.StateMachines.Results
{
    public class StateMachineResult
    {
        public string TechnicalId { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }


        public static StateMachineResult ToDto(StateMachineDefinitionAggregate workflowDefinition)
        {
            return new StateMachineResult
            {
                TechnicalId = workflowDefinition.TechnicalId,
                Id = workflowDefinition.Id,
                CreateDateTime = workflowDefinition.CreateDateTime,
                Description = workflowDefinition.Description,
                Name = workflowDefinition.Name,
                UpdateDateTime = workflowDefinition.UpdateDateTime,
                Version = workflowDefinition.Version,
                ApplicationDomainId = workflowDefinition.ApplicationDomainId,
                Vpn = workflowDefinition.Vpn
            };
        }
    }
}
