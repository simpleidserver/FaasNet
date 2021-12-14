using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.StateMachineInstances.Results
{
    public class InstanceStateResult
    {
        public InstanceStateResult()
        {
            Events = new List<InstanceStateEventResult>();
        }

        public string Id { get; set; }
        public string DefId { get; set; }
        public WorkflowInstanceStateStatus Status { get; set; }
        public JToken Input { get; set; }
        public JToken Output { get; set; }
        public ICollection<InstanceStateEventResult> Events { get; set; }

        public static InstanceStateResult ToDto(WorkflowInstanceState state)
        {
            return new InstanceStateResult
            {
                DefId = state.DefId,
                Events = state.Events.Select(e => InstanceStateEventResult.ToDto(e)).ToList(),
                Id = state.Id,
                Input = state.Input,
                Output = state.Output,
                Status = state.Status
            };
        }
    }
}
