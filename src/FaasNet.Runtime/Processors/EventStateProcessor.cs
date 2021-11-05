using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            await ConsumeEvents(executionContext, eventState, cancellationToken);
            var consumedOnEventIds = GetConsumedEventIndexLst(eventState, executionContext);
            if (consumedOnEventIds.Any())
            {
                var result = new JObject();
                foreach(var consumedOnEventId in consumedOnEventIds)
                {
                    result.Merge(executionContext.StateInstance.BuildOutputEventResult(consumedOnEventId));
                }

                return StateProcessorResult.Ok(result);
            }

            return StateProcessorResult.Block();
        }

        protected async Task ConsumeEvents(WorkflowInstanceExecutionContext executionContext, WorkflowDefinitionEventState eventState, CancellationToken cancellationToken)
        {
            JObject result = new JObject();
            var consumedEvtLst = new List<string>();
            int onEventId = 0;
            foreach (var onEvent in eventState.OnEvents)
            {
                var consumedEvts = onEvent.EventRefs
                    .Select(e => executionContext.WorkflowDef.GetEvent(e))
                    .Where(e => e.Kind == WorkflowDefinitionEventKinds.Consumed);
                foreach (var consumedEvt in consumedEvts)
                {
                    executionContext.Instance.TryListenEvent(executionContext.StateInstance.Id, consumedEvt.Name, consumedEvt.Source, consumedEvt.Type);
                }

                List<OnEventResult> onEventResultLst = new List<OnEventResult>();
                if (!eventState.Exclusive && executionContext.StateInstance.IsAllEvtsConsumed(onEvent.EventRefs))
                {
                    onEventResultLst = await ProcessOnEvent(executionContext, onEvent, consumedEvtLst, cancellationToken);
                }
                else if (eventState.Exclusive)
                {
                    onEventResultLst = await ProcessOnEvent(executionContext, onEvent, consumedEvtLst, cancellationToken);
                }

                if(onEventResultLst.Any())
                {
                    foreach(var onEvtResult in onEventResultLst)
                    {
                        executionContext.Instance.ConsumeOnEventResult(executionContext.StateInstance.Id, onEventId, onEvtResult.EventName, onEvtResult.Result);
                    }
                }

                onEventId++;
            }

            foreach(var consumedEvt in consumedEvtLst)
            {
                var evt = executionContext.WorkflowDef.GetEvent(consumedEvt);
                executionContext.Instance.ProcessEvent(executionContext.StateInstance.Id, evt.Source, evt.Type);
            }
        }

        protected async Task<List<OnEventResult>> ProcessOnEvent(WorkflowInstanceExecutionContext executionContext, WorkflowDefinitionOnEvent onEvent, List<string> consumedEvts, CancellationToken cancellationToken)
        {
            var result = new List<OnEventResult>();
            foreach (var evt in executionContext.StateInstance.GetConsumedEvts(onEvent.EventRefs))
            {
                int index = Array.FindIndex(onEvent.EventRefs.ToArray(), r => r == evt.Name);
                var actions = new List<WorkflowDefinitionAction>
                {
                    onEvent.Actions.ElementAt(index)
                };
                var content = await _actionExecutor.Execute(evt.DataObj, onEvent.ActionMode, actions, executionContext, cancellationToken);
                if (!consumedEvts.Contains(evt.Name))
                {
                    consumedEvts.Add(evt.Name);
                }

                result.Add(new OnEventResult(evt.Name, content));
            }

            return result;
        }

        protected IEnumerable<int> GetConsumedEventIndexLst(WorkflowDefinitionEventState state, WorkflowInstanceExecutionContext executionContext)
        {
            var result = new List<int>();
            for(int i = 0; i < state.OnEvents.Count(); i++)
            {
                var onEvent = state.OnEvents.ElementAt(i);
                if (executionContext.StateInstance.IsAllEvtsProcessed(onEvent.EventRefs))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        protected class OnEventResult
        {
            public OnEventResult(string eventName, JObject result)
            {
                EventName = eventName;
                Result = result;
            }

            public string EventName { get; private set; }
            public JObject Result { get; private set; }
        }
    }
}
