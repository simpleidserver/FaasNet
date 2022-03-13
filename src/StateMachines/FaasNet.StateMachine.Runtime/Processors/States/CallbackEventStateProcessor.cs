using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public class CallbackEventStateProcessor : BaseFlowableStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public CallbackEventStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public override StateMachineDefinitionStateTypes Type => StateMachineDefinitionStateTypes.Callback;

        protected async override Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var callbackState = executionContext.StateDef as StateMachineDefinitionCallbackState;
            await ExecuteOperation(executionContext, callbackState, cancellationToken);
            return ConsumeEvent(executionContext, callbackState, cancellationToken);
        }

        protected async Task ExecuteOperation(StateMachineInstanceExecutionContext executionContext, StateMachineDefinitionCallbackState callbackState, CancellationToken cancellationToken)
        {
            var stateInstance = executionContext.StateInstance;
            if (stateInstance.Status == StateMachineInstanceStateStatus.PENDING)
            {
                return;
            }

            await _actionExecutor.ExecuteAndMerge(stateInstance.Input, StateMachineDefinitionActionModes.Sequential, new List<StateMachineDefinitionAction> { callbackState.Action }, executionContext, cancellationToken);
        }

        protected StateProcessorResult ConsumeEvent(StateMachineInstanceExecutionContext executionContext, StateMachineDefinitionCallbackState callbackState, CancellationToken cancellationToken)
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
