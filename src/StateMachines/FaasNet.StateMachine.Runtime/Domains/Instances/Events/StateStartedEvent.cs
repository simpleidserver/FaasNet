using FaasNet.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State '{stateId}', Status = Started")]
    public class StateStartedEvent : DomainEvent
    {
        public StateStartedEvent(string id, string aggregateId, string stateId, JToken input, DateTime startDateTime) : base(id, aggregateId)
        {
            StateId = stateId;
            Input = input;
            StartDateTime = startDateTime;
        }

        public string StateId { get; set; }
        public JToken Input { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}
