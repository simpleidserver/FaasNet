using FaasNet.Domain;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State instance '{StateInstanceId}' is created")]
    public class StateInstanceCreatedEvent : DomainEvent
    {
        public StateInstanceCreatedEvent(string id, string aggregateId, string stateInstanceId, string defId, DateTime startDateTime) : base(id, aggregateId)
        {
            StateInstanceId = stateInstanceId;
            DefId = defId;
            StartDateTime = startDateTime;
        }

        public string StateInstanceId { get; set; }
        public string DefId { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}
