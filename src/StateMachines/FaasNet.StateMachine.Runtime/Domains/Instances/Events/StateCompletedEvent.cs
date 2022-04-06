using FaasNet.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State {StateId}, Status = Completed")]
    public class StateCompletedEvent : DomainEvent
    {
        public StateCompletedEvent(string id, string aggregateId, string stateId, JToken output, DateTime completedDateTime) : base(id, aggregateId)
        {
            StateId = stateId;
            Output = output;
            CompletedDateTime = completedDateTime;
        }

        public string StateId { get; set; }
        public JToken Output {  get; set; }
        public DateTime CompletedDateTime { get; set; }
    }
}
