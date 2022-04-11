using FaasNet.Domain;
using Newtonsoft.Json;
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
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
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
        public DateTime CompletedDateTime { get; set; }
        public string OutputStr { get; set; }
    }
}
