namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceSequenceFlow
    {
        public string FromStateId { get; set; }
        public string ToStateId { get; set; }

        public static WorkflowInstanceSequenceFlow Create(string fromStateId, string toStateId)
        {
            return new WorkflowInstanceSequenceFlow
            {
                FromStateId = fromStateId,
                ToStateId = toStateId
            };
        }
    }
}
