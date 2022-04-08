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
            var consumedResult = new List<ProcessEventResult>();
            foreach (var onEvent in eventState.OnEvents)
            {
                var consumedEvts = onEvent.EventRefs
                    .Select(e => executionContext.WorkflowDef.GetEvent(e))
                    .Where(e => e.Kind == StateMachineDefinitionEventKinds.Consumed);
                foreach (var consumedEvt in consumedEvts)
                {
                    executionContext.Instance.ListenEvt(executionContext.StateInstance.Id, consumedEvt.Name, consumedEvt.Source, consumedEvt.Type, consumedEvt.Topic);
                }

                List<ProcessEventResult> evtResultLst = new List<ProcessEventResult>();
                if (!eventState.Exclusive && executionContext.StateInstance.IsAllEvtsConsumed(onEvent.EventRefs))
                {
                    evtResultLst = await ProcessOnEvent(executionContext, onEvent, cancellationToken);
                }
                else if (eventState.Exclusive)
                {
                    evtResultLst = await ProcessOnEvent(executionContext, onEvent, cancellationToken);
                }

                consumedResult.AddRange(evtResultLst);
            }

            foreach(var consumedEvt in consumedResult)
            {
                executionContext.Instance.ProcessEvent(executionContext.StateInstance.Id, consumedEvt.EvtName, consumedEvt.Result.ToString());
            }
        }

        protected async Task<List<ProcessEventResult>> ProcessOnEvent(StateMachineInstanceExecutionContext executionContext, StateMachineDefinitionOnEvent onEvent, CancellationToken cancellationToken)
        {
            var result = new List<ProcessEventResult>();
            foreach (var evt in executionContext.StateInstance.GetConsumedEvts(onEvent.EventRefs))
            {
                int index = Array.FindIndex(onEvent.EventRefs.ToArray(), r => r == evt.Name);
                var actions = new List<StateMachineDefinitionAction>
                {
                    onEvent.Actions.ElementAt(index)
                };
                var content = await _actionExecutor.ExecuteAndMerge(evt.InputDataObj, onEvent.ActionMode, actions, executionContext, cancellationToken);
                result.Add(new ProcessEventResult(evt.Name, content));
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
                        var data = processedEvt.OutputDataObj;
                        output.Merge(data, onEvent.EventDataFilter?.Data, onEvent.EventDataFilter?.ToStateData);
                    }
                }
            }

            return result;
        }

        protected class ProcessEventResult
        {
            public ProcessEventResult(string evtName, JToken result)
            {
                EvtName = evtName;
                Result = result;
            }

            public string EvtName { get; private set; }
            public JToken Result { get; private set; }
        }
    }
}
