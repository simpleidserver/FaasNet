using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;

namespace FaasNet.Gateway.Core.StateMachines.Results
{
    public class StateMachineInstanceDetails
    {
        public string Id { get; set; }
        public string StateMachine { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public string Output { get; set; }

        public static StateMachineInstanceDetails Build(WorkflowInstanceAggregate workflowInstance)
        {
            return new StateMachineInstanceDetails
            {
                Id = workflowInstance.Id,
                Output = workflowInstance.OutputStr,
                StateMachine = workflowInstance.WorkflowDefId,
                Status = workflowInstance.Status
            };
        }
    }
}
