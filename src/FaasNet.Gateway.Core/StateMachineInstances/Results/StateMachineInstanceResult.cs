using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using System;

namespace FaasNet.Gateway.Core.StateMachineInstances.Results
{
    public class StateMachineInstanceResult
    {
        public string Id { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefTechnicalId { get; set; }
        public string WorkflowDefName { get; set; }
        public string WorkflowDefDescription { get; set; }
        public int WorkflowDefVersion { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public DateTime CreateDateTime { get; set; }

        public static StateMachineInstanceResult ToDto(WorkflowInstanceAggregate workflowInstance)
        {
            return new StateMachineInstanceResult
            {
                Id = workflowInstance.Id,
                WorkflowDefVersion = workflowInstance.WorkflowDefVersion,
                Status = workflowInstance.Status,
                WorkflowDefTechnicalId = workflowInstance.WorkflowDefTechnicalId,
                WorkflowDefId = workflowInstance.WorkflowDefId,
                WorkflowDefName = workflowInstance.WorkflowDefName,
                WorkflowDefDescription = workflowInstance.WorkflowDefDescription,
                CreateDateTime = workflowInstance.CreateDateTime
            };
        }
    }
}
