using FaasNet.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("StateMachine {AggregateId}, Status = Terminated")]
    public class StateMachineTerminatedEvent : DomainEvent
    {
        public StateMachineTerminatedEvent(string id, string aggregateId, JToken output, DateTime terminateDateTime) : base(id, aggregateId)
        {
            Output = output;
            TerminateDateTime = terminateDateTime;
        }

        [JsonIgnore]
        public JToken Output
        {
            get
            {
                return string.IsNullOrWhiteSpace(OutputStr) ? null : JToken.Parse(OutputStr);
            }
            set
            {
                if (value != null)
                {
                    OutputStr = value.ToString();
                    return;
                }

                OutputStr = null;
            }
        }
        public DateTime TerminateDateTime { get; set; }
        public string OutputStr { get; set; }
    }
}
