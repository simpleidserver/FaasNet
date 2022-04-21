using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceStateHistory : ICloneable
    {
        #region Properties

        public StateMachineInstanceStateStatus Status { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Data { get; set; }

        #endregion

        public static StateMachineInstanceStateHistory Create(StateMachineInstanceStateStatus status, DateTime startDateTime, string data = null) 
        {
            return new StateMachineInstanceStateHistory
            {
                Data = data,
                Status = status,
                StartDateTime = startDateTime
            };
        }

        public object Clone()
        {
            return new StateMachineInstanceStateHistory
            {
                Data = Data,
                Status = Status,
                StartDateTime = StartDateTime,
                EndDateTime = EndDateTime
            };
        }
    }
}
