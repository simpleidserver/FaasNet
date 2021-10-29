using FaasNet.Runtime.Domains;

namespace FaasNet.Runtime
{
    public class WorkflowInstanceExecutionContext
    {
        public WorkflowInstanceExecutionContext(BaseWorkflowDefinitionState stateDef, WorkflowInstanceState stateInstance, WorkflowDefinitionAggregate workflowDef)
        {
            StateDef = stateDef;
            StateInstance = stateInstance;
            WorkflowDef = workflowDef;
        }

        public BaseWorkflowDefinitionState StateDef { get; private set; }
        public WorkflowInstanceState StateInstance { get; set; }
        public WorkflowDefinitionAggregate WorkflowDef { get; private set; }
    }
}
