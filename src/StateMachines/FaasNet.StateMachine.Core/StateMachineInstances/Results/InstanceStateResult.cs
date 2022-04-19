using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Results
{
    public class InstanceStateResult
    {
        public InstanceStateResult()
        {
            Events = new List<InstanceStateEventResult>();
            Histories = new List<InstanceStateHistoryResult>();
        }

        public string Id { get; set; }
        public string DefId { get; set; }
        public StateMachineInstanceStateStatus Status { get; set; }
        public JToken Input { get; set; }
        public JToken Output { get; set; }
        public ICollection<InstanceStateEventResult> Events { get; set; }
        public ICollection<InstanceStateHistoryResult> Histories { get; set; }

        public static InstanceStateResult ToDto(StateMachineInstanceState state)
        {
            return new InstanceStateResult
            {
                DefId = state.DefId,
                Events = state.Events.Select(e => InstanceStateEventResult.ToDto(e)).ToList(),
                Id = state.Id,
                Input = state.GetInput(),
                Output = state.GetOutput(),
                Status = state.Status,
                Histories = state.Histories.Select(h => InstanceStateHistoryResult.ToDto(h)).ToList()
            };
        }
    }
}
