using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public class OperationStateProcessor : IStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public OperationStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Operation;

        public async Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var operationState = executionContext.StateDef as WorkflowDefinitionOperationState;
            var result = await _actionExecutor.Execute(executionContext.StateInstance.Input, executionContext.StateInstance.Input, operationState.ActionMode, operationState.Actions, executionContext, cancellationToken);
            return StateProcessorResult.Ok(result);
        }
    }
}
