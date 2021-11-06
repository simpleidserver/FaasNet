using System;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceSequenceFlow : ICloneable
    {
        #region Properties

        public string FromStateId { get; set; }
        public string ToStateId { get; set; }

        #endregion

        public static WorkflowInstanceSequenceFlow Create(string fromStateId, string toStateId)
        {
            return new WorkflowInstanceSequenceFlow
            {
                FromStateId = fromStateId,
                ToStateId = toStateId
            };
        }

        public object Clone()
        {
            return new WorkflowInstanceSequenceFlow
            {
                FromStateId = FromStateId,
                ToStateId = ToStateId
            };
        }
    }
}
