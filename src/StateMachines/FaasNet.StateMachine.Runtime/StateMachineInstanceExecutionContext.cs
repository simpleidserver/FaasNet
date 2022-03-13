using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;

namespace FaasNet.StateMachine.Runtime
{
    public class StateMachineInstanceExecutionContext
    {
        public StateMachineInstanceExecutionContext(BaseStateMachineDefinitionState stateDef, StateMachineInstanceState stateInstance, StateMachineInstanceAggregate instance, StateMachineDefinitionAggregate workflowDef)
        {
            StateDef = stateDef;
            StateInstance = stateInstance;
            Instance = instance;
            WorkflowDef = workflowDef;
        }

        public BaseStateMachineDefinitionState StateDef { get; private set; }
        public StateMachineInstanceState StateInstance { get; set; }
        public StateMachineInstanceAggregate Instance { get; set; }
        public StateMachineDefinitionAggregate WorkflowDef { get; private set; }
    }
}
