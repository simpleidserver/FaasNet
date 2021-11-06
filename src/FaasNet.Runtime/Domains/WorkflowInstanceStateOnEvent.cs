using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceStateOnEvent : ICloneable
    {
        #region Properties

        public int OnEventId { get; set; }
        public string EventName { get; set; }
        public string DataStr { get; set; }
        public JObject Data
        {
            get
            {
                return JObject.Parse(DataStr);
            }
        }

        #endregion

        public static WorkflowInstanceStateOnEvent Create(int onEventId, string eventName, string dataStr)
        {
            return new WorkflowInstanceStateOnEvent
            {
                DataStr = dataStr,
                EventName = eventName,
                OnEventId = onEventId
            };
        }

        public object Clone()
        {
            return new WorkflowInstanceStateOnEvent
            {
                DataStr = DataStr,
                EventName = EventName,
                OnEventId = OnEventId
            };
        }
    }
}
