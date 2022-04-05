using FaasNet.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("StateMachine '{AggregateId}', Status = Terminated")]
    public class StateMachineTerminatedEvent : DomainEvent
    {
        public StateMachineTerminatedEvent(string id, string aggregateId, JToken output, DateTime terminateDateTime) : base(id, aggregateId)
        {
            Output = output;
            TerminateDateTime = terminateDateTime;
        }

        public JToken Output { get; set; }
        public DateTime TerminateDateTime { get; set; }
    }
}
