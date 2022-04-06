using FaasNet.Domain;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State {StateId}, Status = Blocked")]
    public class StateBlockedEvent : DomainEvent
    {
        public StateBlockedEvent(string id, string aggregateId, string stateId, DateTime blockedDateTime) : base(id, aggregateId)
        {
            StateId = stateId;
            BlockedDateTime = blockedDateTime;
        }

        public string StateId { get; set; }
        public DateTime BlockedDateTime { get; set; }
    }
}
