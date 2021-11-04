using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceStateEvent
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public WorkflowInstanceStateEventStates State { get; set; }
        public string Data { get; set; }
        public JObject DataObj
        {
            get
            {
                return JObject.Parse(Data);
            }
        }

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
    }
}
