using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public class EventStateProcessor : BaseFlowableStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public EventStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public override StateMachineDefinitionStateTypes Type => StateMachineDefinitionStateTypes.Event;

        protected override async Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var eventState = executionContext.StateDef as StateMachineDefinitionEventState;
            await ConsumeEvents(executionContext, eventState, cancellationToken);
            if (TryGetOutput(eventState, executionContext, out JToken output))
            {
                return Ok(output, eventState);
            }

            return StateProcessorResult.Block();
        }

        protected async Task ConsumeEvents(StateMachineInstanceExecutionContext executionContext, StateMachineDefinitionEventState eventState, CancellationToken cancellationToken)
        {
            JObject result = new JObject();
            var consumedEvtLst = new List<string>();
            int i = 0;
            foreach (var onEvent in eventState.OnEvents)
            {
                var consumedEvts = onEvent.EventRefs
                    .Select(e => executionContext.WorkflowDef.GetEvent(e))
                    .Where(e => e.Kind == StateMachineDefinitionEventKinds.Consumed);
                foreach (var consumedEvt in consumedEvts)
                {
                    executionContext.Instance.ListenEvt(executionContext.StateInstance.Id, consumedEvt.Name, consumedEvt.Source, consumedEvt.Type);
                }

                List<OnEventResult> evtResultLst = null;
                if (!eventState.Exclusive && executionContext.StateInstance.IsAllEvtsConsumed(onEvent.EventRefs))
                {
                    evtResultLst = await ProcessOnEvent(executionContext, onEvent, consumedEvtLst, cancellationToken);
                }
                else if (eventState.Exclusive)
                {
                    evtResultLst = await ProcessOnEvent(executionContext, onEvent, consumedEvtLst, cancellationToken);
                }

                if(evtResultLst != null && evtResultLst.Any())
                {
                    foreach(var evtResult in evtResultLst)
                    {
                        executionContext.Instance.ConsumeEvt(
                            executionContext.StateInstance.Id, 
                            evtResult.Source, 
                            evtResult.Type,
                            i, 
                            evtResult.Result.ToString());
                    }
                }

                i++;
            }

            foreach(var consumedEvt in consumedEvtLst)
            {
                var evt = executionContext.WorkflowDef.GetEvent(consumedEvt);
                executionContext.Instance.ProcessEvent(executionContext.StateInstance.Id, evt.Source, evt.Type);
            }
        }

        protected async Task<List<OnEventResult>> ProcessOnEvent(StateMachineInstanceExecutionContext executionContext, StateMachineDefinitionOnEvent onEvent, List<string> consumedEvts, CancellationToken cancellationToken)
        {
            var result = new List<OnEventResult>();
            foreach (var evt in executionContext.StateInstance.GetConsumedEvts(onEvent.EventRefs))
            {
                int index = Array.FindIndex(onEvent.EventRefs.ToArray(), r => r == evt.Name);
                var actions = new List<StateMachineDefinitionAction>
                {
                    onEvent.Actions.ElementAt(index)
                };
                var content = await _actionExecutor.ExecuteAndMerge(evt.InputDataObj, onEvent.ActionMode, actions, executionContext, cancellationToken);
                if (!consumedEvts.Contains(evt.Name))
                {
                    consumedEvts.Add(evt.Name);
                }

                result.Add(new OnEventResult(evt.Source, evt.Type, content));
            }

            return result;
        }

        protected bool TryGetOutput(StateMachineDefinitionEventState state, StateMachineInstanceExecutionContext executionContext, out JToken output)
        {
            var result = false;
            output = executionContext.StateInstance.Input;
            for(int i = 0; i < state.OnEvents.Count; i++)
            {
                var onEvent = state.OnEvents.ElementAt(i);
                if (executionContext.StateInstance.IsAllEvtsProcessed(onEvent.EventRefs))
                {
                    result = true;
                    if (onEvent.EventDataFilter != null && !onEvent.EventDataFilter.UseData)
                    {
                        continue;
                    }

                    foreach (var processedEvt in executionContext.StateInstance.GetProcessedEvts(onEvent.EventRefs))
                    {
                        var data = processedEvt.OutputLst.ElementAt(i).DataObj;
                        output.Merge(data, onEvent.EventDataFilter?.Data, onEvent.EventDataFilter?.ToStateData);
                    }
                }
            }

            return result;
        }

        protected class OnEventResult
        {
            public OnEventResult(string source, string type, JToken result)
            {
                Source = source;
                Type = type;
                Result = result;
            }

            public string Source { get; private set; }
            public string Type { get; private set; }
            public JToken Result { get; private set; }
        }
    }
}
