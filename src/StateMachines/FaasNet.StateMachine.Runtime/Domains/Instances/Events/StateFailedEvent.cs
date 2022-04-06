using FaasNet.Domain;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State {StateId}, Status = Failed")]
    public class StateFailedEvent : DomainEvent
    {
        public StateFailedEvent(string id, string aggregateId, string stateId, string exception, DateTime failedDateTime) : base(id, aggregateId)
        {
            StateId = stateId;
            Exception = exception;
            FailedDateTime = failedDateTime;
        }

        public string StateId { get; set; }
        public string Exception { get; set; }
        public DateTime FailedDateTime { get; set; }
    }
}
