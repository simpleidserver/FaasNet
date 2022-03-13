using FaasNet.StateMachine.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceStateEvent : ICloneable
    {
        public StateMachineInstanceStateEvent()
        {
            OutputLst = new List<StateMachineInstanceStateEventOutput>();
        }

        #region Properties

        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public StateMachineInstanceStateEventStates State { get; set; }
        public string InputData { get; set; }
        public JToken InputDataObj
        {
            get
            {
                return JToken.Parse(InputData);
            }
        }
        public virtual ICollection<StateMachineInstanceStateEventOutput> OutputLst { get; set; }

        #endregion

        public static StateMachineInstanceStateEvent Create(string name, string source, string type)
        {
            return new StateMachineInstanceStateEvent
            {
                Name = name,
                Source = source,
                Type = type,
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
                OutputLst = OutputLst.Select(o => (StateMachineInstanceStateEventOutput)o.Clone()).ToList()
            };
        }
    }
}
