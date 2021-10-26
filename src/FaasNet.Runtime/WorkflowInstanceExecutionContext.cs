using FaasNet.Runtime.Domains;

namespace FaasNet.Runtime
{
    public class WorkflowInstanceExecutionContext
    {
        public WorkflowInstanceExecutionContext(BaseWorkflowDefinitionState stateDef)
        {
            StateDef = stateDef;
        }

        public BaseWorkflowDefinitionState StateDef { get; private set; }
    }
}
