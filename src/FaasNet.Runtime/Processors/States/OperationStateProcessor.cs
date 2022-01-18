using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public class OperationStateProcessor : BaseFlowableStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public OperationStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public override WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Operation;

        protected override async Task<StateProcessorResult> Handle(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var operationState = executionContext.StateDef as WorkflowDefinitionOperationState;
            var result = await _actionExecutor.ExecuteAndMerge(executionContext.StateInstance.Input, operationState.ActionMode, operationState.Actions, executionContext, cancellationToken);
            return Ok(result, operationState);
        }
    }
}
