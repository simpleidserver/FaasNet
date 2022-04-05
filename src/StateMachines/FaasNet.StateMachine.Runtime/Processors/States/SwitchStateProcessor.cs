using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public class SwitchStateProcessor : BaseStateProcessor
    {
        public SwitchStateProcessor()
        {

        }

        public override StateMachineDefinitionStateTypes Type => StateMachineDefinitionStateTypes.Switch;

        protected override Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var swichState = executionContext.StateDef as StateMachineDefinitionSwitchState;
            if (swichState.Conditions.Any(c => c.ConditionType == StateMachineDefinitionEventConditionTypes.DATA))
            {
                if (TryCheck(executionContext, swichState.Conditions.Cast<StateMachineDefinitionSwitchDataCondition>().ToList(), out StateMachineDefinitionSwitchDataCondition result))
                {
                    return Task.FromResult(Ok(result, executionContext.StateInstance.Input));
                }
            }
            else
            {
                if (TryCheck(executionContext, swichState.Conditions.Cast<StateMachineDefinitionSwitchEventCondition>().ToList(), out StateMachineDefinitionSwitchEventCondition result, out StateMachineInstanceStateEvent stateEvt))
                {
                    executionContext.Instance.ProcessEvent(executionContext.StateInstance.Id, stateEvt.Source, stateEvt.Type);
                    var output = executionContext.StateInstance.Input;
                    if (result.EventDataFilter != null && result.EventDataFilter.UseData)
                    {
                        output.Merge(stateEvt.InputDataObj, result.EventDataFilter.Data, result.EventDataFilter.ToStateData);
                    }

                    return Task.FromResult(Ok(result, output));
                }
            }

            return Task.FromResult(StateProcessorResult.Block());
        }

        protected StateProcessorResult Ok(BaseEventCondition evtCondition, JToken result)
        {
            if (evtCondition.End)
            {
                return StateProcessorResult.End(result);
            }

            return StateProcessorResult.Next(result, evtCondition.Transition);
        }

        protected bool TryCheck(StateMachineInstanceExecutionContext executionContext, List<StateMachineDefinitionSwitchDataCondition> conditions, out StateMachineDefinitionSwitchDataCondition result)
        {
            result = null;
            foreach(var condition in conditions.Cast<StateMachineDefinitionSwitchDataCondition>())
            {
                if (executionContext.StateInstance.EvaluateCondition(condition.Condition))
                {
                    result = condition;
                    return true;
                }
            }

            return false;
        }

        protected bool TryCheck(StateMachineInstanceExecutionContext executionContext, List<StateMachineDefinitionSwitchEventCondition> conditions, out StateMachineDefinitionSwitchEventCondition result, out StateMachineInstanceStateEvent stateEvt)
        {
            result = null;
            stateEvt = null;
            foreach(var condition in conditions)
            {
                var evtRef = condition.EventRef;
                var evt = executionContext.WorkflowDef.GetEvent(evtRef);
                executionContext.Instance.ListenEvt(executionContext.StateInstance.Id, evt.Name, evt.Source, evt.Type);
                if (executionContext.StateInstance.IsEvtConsumed(evt.Name))
                {
                    var consumedEvt = executionContext.StateInstance.GetConsumedEvt(evt.Name);
                    stateEvt = consumedEvt;
                    result = condition;
                    return true;
                }
            }

            return false;
        }
    }
}
