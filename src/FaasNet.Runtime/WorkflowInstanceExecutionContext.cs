using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Instances;

namespace FaasNet.Runtime
{
    public class WorkflowInstanceExecutionContext
    {
        public WorkflowInstanceExecutionContext(BaseWorkflowDefinitionState stateDef, WorkflowInstanceState stateInstance, WorkflowInstanceAggregate instance, WorkflowDefinitionAggregate workflowDef)
        {
            StateDef = stateDef;
            StateInstance = stateInstance;
            Instance = instance;
            WorkflowDef = workflowDef;
        }

        public BaseWorkflowDefinitionState StateDef { get; private set; }
        public WorkflowInstanceState StateInstance { get; set; }
        public WorkflowInstanceAggregate Instance { get; set; }
        public WorkflowDefinitionAggregate WorkflowDef { get; private set; }
    }
}
