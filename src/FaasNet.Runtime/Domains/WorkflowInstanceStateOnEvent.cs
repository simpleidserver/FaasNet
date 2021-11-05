using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceStateOnEvent
    {
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

        public static WorkflowInstanceStateOnEvent Create(int onEventId, string eventName, string dataStr)
        {
            return new WorkflowInstanceStateOnEvent
            {
                DataStr = dataStr,
                EventName = eventName,
                OnEventId = onEventId
            };
        }
    }
}
