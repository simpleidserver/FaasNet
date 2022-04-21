using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using System;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Results
{
    public class InstanceStateHistoryResult
    {
        public StateMachineInstanceStateStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }

        public static InstanceStateHistoryResult ToDto(StateMachineInstanceStateHistory history)
        {
            return new InstanceStateHistoryResult
            {
                Status = history.Status,
                Data = history.Data,
                Timestamp = history.StartDateTime
            };
        }
    }
}
