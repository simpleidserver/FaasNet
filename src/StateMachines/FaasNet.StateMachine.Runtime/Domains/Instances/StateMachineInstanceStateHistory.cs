using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceStateHistory : ICloneable
    {
        #region Properties

        public StateMachineInstanceStateStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }

        #endregion

        public static StateMachineInstanceStateHistory Create(StateMachineInstanceStateStatus status, DateTime timestamp, string data = null) 
        {
            return new StateMachineInstanceStateHistory
            {
                Data = data,
                Status = status,
                Timestamp = timestamp
            };
        }

        public object Clone()
        {
            return new StateMachineInstanceStateHistory
            {
                Data = Data,
                Status = Status,
                Timestamp = Timestamp
            };
        }
    }
}
