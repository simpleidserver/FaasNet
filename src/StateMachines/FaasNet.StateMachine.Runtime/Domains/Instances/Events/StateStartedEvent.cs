using FaasNet.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State {StateId}, Status = Started")]
    public class StateStartedEvent : DomainEvent
    {
        public StateStartedEvent(string id, string aggregateId, string stateId, JToken input, DateTime startDateTime) : base(id, aggregateId)
        {
            StateId = stateId;
            Input = input;
            StartDateTime = startDateTime;
        }

        public string StateId { get; set; }
        [JsonIgnore]
        public JToken Input
        {
            get
            {
                return string.IsNullOrWhiteSpace(InputStr) ? null : JToken.Parse(InputStr);
            }
            set
            {
                if(value != null)
                {
                    InputStr = value.ToString();
                    return;
                }

                InputStr = null;
            }
        }
        public DateTime StartDateTime { get; set; }
        public string InputStr { get; set; }
    }
}
