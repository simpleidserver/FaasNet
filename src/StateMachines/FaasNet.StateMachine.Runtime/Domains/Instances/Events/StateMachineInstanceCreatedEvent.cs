using FaasNet.Domain;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State machine instance is created")]
    public class StateMachineInstanceCreatedEvent : DomainEvent
    {
        public StateMachineInstanceCreatedEvent(string id, string aggregateId, string workflowDefTechnicalId, string workflowDefId, string workflowDefName, string workflowDefDescription, int workflowDefVersion, string vpn, string serializedDefinition, DateTime createDateTime) : base(id, aggregateId)
        {
            WorkflowDefTechnicalId = workflowDefTechnicalId;
            WorkflowDefId = workflowDefId;
            WorkflowDefName = workflowDefName;
            WorkflowDefDescription = workflowDefDescription;
            WorkflowDefVersion = workflowDefVersion;
            Vpn = vpn;
            SerializedDefinition = serializedDefinition;
            CreateDateTime = createDateTime;
        }

        public string WorkflowDefTechnicalId { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefName { get; set; }
        public string WorkflowDefDescription { get; set; }
        public int WorkflowDefVersion { get; set; }
        public string Vpn { get; set; }
        public string SerializedDefinition { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
