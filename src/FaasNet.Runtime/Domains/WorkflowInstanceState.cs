using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceState
    {
        public string Id { get; set; }
        public string DefId { get; set; }
        public WorkflowInstanceStateStatus Status { get; set; }
        public string InputStr { get; set; }
        public JObject Input
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InputStr))
                {
                    return null;
                }

                return JObject.Parse(InputStr);
            }
        }
        public string OutputStr { get; set; }
        public JObject Output
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OutputStr))
                {
                    return null;
                }

                return JObject.Parse(OutputStr);
            }
        }

        public void Start(string input)
        {
            InputStr = input;
            Status = WorkflowInstanceStateStatus.ACTIVE;
        }

        public void Complete(string output)
        {
            OutputStr = output;
            Status = WorkflowInstanceStateStatus.COMPLETE;
        }

        public static WorkflowInstanceState Create(string defId)
        {
            return new WorkflowInstanceState
            {
                Id = Guid.NewGuid().ToString(),
                DefId = defId,
                Status = WorkflowInstanceStateStatus.CREATE
            };
        }
    }
}
