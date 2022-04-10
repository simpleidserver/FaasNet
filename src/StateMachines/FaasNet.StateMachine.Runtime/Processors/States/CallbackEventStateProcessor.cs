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
            var evtRef = callbackState.EventRef;
            var evtDef = executionContext.WorkflowDef.GetEvent(evtRef);
            executionContext.Instance.ListenEvt(executionContext.StateInstance.Id, evtDef.Name, evtDef.Source, evtDef.Type, evtDef.Topic);
            var consumedEvts = executionContext.StateInstance.GetConsumedEvts(new string[] { evtRef });
            if (consumedEvts.Any())
            {
                var consumedEvt = consumedEvts.First();
                var output = ApplyEventDataFilter(executionContext.StateInstance.Input, callbackState.EventDataFilter, consumedEvt);
                var result = await _actionExecutor.ExecuteAndMerge(output, StateMachineDefinitionActionModes.Sequential, new List<StateMachineDefinitionAction> { callbackState.Action }, executionContext, cancellationToken);
                return Ok(result, callbackState);
            }

            return StateProcessorResult.Block();
        }
    }
}
