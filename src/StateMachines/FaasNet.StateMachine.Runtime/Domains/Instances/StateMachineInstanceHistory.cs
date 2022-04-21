using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceHistory : ICloneable
    {
        public StateMachineInstanceStatus Status { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public static StateMachineInstanceHistory Create(StateMachineInstanceStatus status, DateTime startDateTime)
        {
            return new StateMachineInstanceHistory
            {
                Status = status,
                StartDateTime = startDateTime
            };
        }

        public object Clone()
        {
            return new StateMachineInstanceHistory
            {
                EndDateTime = EndDateTime,
                Status = Status,
                StartDateTime = StartDateTime
            };
        }
    }
}
