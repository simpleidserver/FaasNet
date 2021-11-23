using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Gateway.Core.StateMachineInstances.Results
{
    public class StateMachineInstanceResult
    {
        public string Id { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public JObject Output { get; set; }

        public static StateMachineInstanceResult Build(WorkflowInstanceAggregate instance)
        {
            return new StateMachineInstanceResult
            {
                Id = instance.Id,
                CreateDateTime = instance.CreateDateTime,
                Output = instance.Output,
                Status = instance.Status,
                WorkflowDefId = instance.WorkflowDefId,
                WorkflowDefVersion = instance.WorkflowDefVersion
            };
        }
    }
}
