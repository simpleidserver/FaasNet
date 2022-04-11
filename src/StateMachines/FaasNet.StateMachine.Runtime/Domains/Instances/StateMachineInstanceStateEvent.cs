using FaasNet.StateMachine.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json.Serialization;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceStateEvent : ICloneable
    {
        #region Properties

        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Topic { get; set; }
        public StateMachineInstanceStateEventStates State { get; set; }
        public string InputData { get; set; }
        public JToken GetInputDataObj()
        {
            return JToken.Parse(InputData);
        }
        public string OutputData { get; set; }
        public JToken GetOutputDataObj()
        {
            return string.IsNullOrWhiteSpace(OutputData) ? null : JToken.Parse(OutputData);
        }

        #endregion

        public static StateMachineInstanceStateEvent Create(string name, string source, string type, string topic)
        {
            return new StateMachineInstanceStateEvent
            {
                Name = name,
                Source = source,
                Type = type,
                Topic = topic,
                State = StateMachineInstanceStateEventStates.CREATED
            };
        }

        public object Clone()
        {
            return new StateMachineInstanceStateEvent
            {
                InputData = InputData,
                Name = Name,
                Source = Source,
                State = State,
                Type = Type,
                Topic = Topic,
                OutputData = OutputData
            };
        }
    }
}
