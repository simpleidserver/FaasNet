using FaasNet.Domain;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("Reactivate state machine instance {AggregateId}")]
    public class StateMachineInstanceReactivatedEvent : DomainEvent
    {
        public StateMachineInstanceReactivatedEvent(string id, string aggregateId, DateTime reactivationDateTime) : base(id, aggregateId)
        {
            ReactivationDateTime = reactivationDateTime;
        }

        public DateTime ReactivationDateTime { get; set; }
    }
}
