using FaasNet.Runtime.Domains.Enums;
using System;

namespace FaasNet.Runtime.Domains.Instances
{
    public class WorkflowInstanceStateHistory : ICloneable
    {
        #region Properties

        public WorkflowInstanceStateStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }

        #endregion

        public static WorkflowInstanceStateHistory Create(WorkflowInstanceStateStatus status, DateTime timestamp, string data = null) 
        {
            return new WorkflowInstanceStateHistory
            {
                Data = data,
                Status = status,
                Timestamp = timestamp
            };
        }

        public object Clone()
        {
            return new WorkflowInstanceStateHistory
            {
                Data = Data,
                Status = Status,
                Timestamp = Timestamp
            };
        }
    }
}
