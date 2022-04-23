using FaasNet.Domain;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("Reactivate the state {StateId}")]
    public class StateReactivatedEvent : DomainEvent
    {
        public StateReactivatedEvent(string id, string aggregateId, string stateId, DateTime startDateTime) : base(id, aggregateId)
        {
            StateId = stateId;
            StartDateTime = startDateTime;
        }

        public string StateId { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}
