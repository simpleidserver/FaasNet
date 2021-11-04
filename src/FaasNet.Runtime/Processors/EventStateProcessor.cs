using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.Resources;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public class EventStateProcessor : IStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public EventStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Event;

        public async Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            // Si c'est le state de départ alors il faut créer une nouvelle instance à chaque fois.
            // OnEvent lorsque success alors on peut passer au state suivant.
            var eventState = executionContext.StateDef as WorkflowDefinitionEventState;
            var outputState = executionContext.StateInstance.Input;
            await ConsumeEvents(executionContext, eventState, cancellationToken);
            if (eventState.OnEvents.Any(e => executionContext.StateInstance.IsAllEvtsProcessed(e.EventRefs)))
            {
                return StateProcessorResult.Ok(outputState);
            }

            return StateProcessorResult.Block();
        }

        protected async Task ConsumeEvents(WorkflowInstanceExecutionContext executionContext, WorkflowDefinitionEventState eventState, CancellationToken cancellationToken)
        {
            foreach (var onEvent in eventState.OnEvents)
            {
                var consumedEvts = onEvent.EventRefs.Select(e => executionContext.WorkflowDef.GetEvent(e)).Where(e => e.Kind == WorkflowDefinitionEventKinds.Consumed);
                foreach (var consumedEvt in consumedEvts)
                {
                    executionContext.Instance.TryListenEvent(executionContext.StateInstance.Id, consumedEvt.Name, consumedEvt.Source, consumedEvt.Type);
                }

                if (!eventState.Exclusive && executionContext.StateInstance.IsAllEvtsConsumed())
                {
                    // TODO : Merge tous les résultats !!!!
                    await _actionExecutor.Execute(null, onEvent.ActionMode, onEvent.Actions, executionContext, cancellationToken);
                    executionContext.Instance.ProcessAllEvents(executionContext.StateInstance.Id);
                    continue;
                }

                if (eventState.Exclusive)
                {
                    foreach (var evt in executionContext.StateInstance.GetConsumedEvts())
                    {
                        var input = evt.DataObj;
                        await _actionExecutor.Execute(evt.DataObj, onEvent.ActionMode, onEvent.Actions, executionContext, cancellationToken);
                        executionContext.Instance.ProcessEvent(executionContext.StateInstance.Id, evt.Source, evt.Type);
                    }
                }
            }
        }
    }
}
