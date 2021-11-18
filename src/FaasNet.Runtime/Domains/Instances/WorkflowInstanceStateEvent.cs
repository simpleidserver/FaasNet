using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Domains.Instances
{
    public class WorkflowInstanceStateEvent : ICloneable
    {
        public WorkflowInstanceStateEvent()
        {
            OutputLst = new List<WorkflowInstanceStateEventOutput>();
        }

        #region Properties

        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public WorkflowInstanceStateEventStates State { get; set; }
        public string InputData { get; set; }
        public JObject InputDataObj
        {
            get
            {
                return JObject.Parse(InputData);
            }
        }
        public virtual ICollection<WorkflowInstanceStateEventOutput> OutputLst { get; set; }

        #endregion

        public static WorkflowInstanceStateEvent Create(string name, string source, string type)
        {
            return new WorkflowInstanceStateEvent
            {
                Name = name,
                Source = source,
                Type = type,
                State = WorkflowInstanceStateEventStates.CREATED
            };
        }

        public object Clone()
        {
            return new WorkflowInstanceStateEvent
            {
                InputData = InputData,
                Name = Name,
                Source = Source,
                State = State,
                Type = Type,
                OutputLst = OutputLst.Select(o => (WorkflowInstanceStateEventOutput)o.Clone()).ToList()
            };
        }
    }
}
