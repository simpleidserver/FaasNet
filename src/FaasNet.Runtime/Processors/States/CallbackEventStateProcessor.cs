using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public class CallbackEventStateProcessor : BaseFlowableStateProcessor, IStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public CallbackEventStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Callback;

        public async Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var callbackState = executionContext.StateDef as WorkflowDefinitionCallbackState;
            await ExecuteOperation(executionContext, callbackState, cancellationToken);
            return ConsumeEvent(executionContext, callbackState, cancellationToken);
        }

        protected async Task ExecuteOperation(WorkflowInstanceExecutionContext executionContext, WorkflowDefinitionCallbackState callbackState, CancellationToken cancellationToken)
        {
            var stateInstance = executionContext.StateInstance;
            if (stateInstance.Status == WorkflowInstanceStateStatus.PENDING)
            {
                return;
            }

            await _actionExecutor.ExecuteAndMerge(stateInstance.Input, WorkflowDefinitionActionModes.Sequential, new List<WorkflowDefinitionAction> { callbackState.Action }, executionContext, cancellationToken);
        }

        protected StateProcessorResult ConsumeEvent(WorkflowInstanceExecutionContext executionContext, WorkflowDefinitionCallbackState callbackState, CancellationToken cancellationToken)
        {
            var evtRef = callbackState.EventRef;
            var evtDef = executionContext.WorkflowDef.GetEvent(evtRef);
            executionContext.Instance.TryListenEvent(executionContext.StateInstance.Id, evtDef.Name, evtDef.Source, evtDef.Type);
            var consumedEvts = executionContext.StateInstance.GetConsumedEvts(new string[] { evtRef });
            if (consumedEvts.Any())
            {
                var consumedEvt = consumedEvts.First();
                return Ok(consumedEvt.InputDataObj, callbackState);
            }

            return StateProcessorResult.Block();
        }
    }
}
