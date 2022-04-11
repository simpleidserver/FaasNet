using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public class OperationStateProcessor : BaseFlowableStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public OperationStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public override StateMachineDefinitionStateTypes Type => StateMachineDefinitionStateTypes.Operation;

        protected override async Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var operationState = executionContext.StateDef as StateMachineDefinitionOperationState;
            var result = await _actionExecutor.ExecuteAndMerge(executionContext.StateInstance.GetInput(), operationState.ActionMode, operationState.Actions, executionContext, cancellationToken);
            return Ok(result, operationState);
        }
    }
}
