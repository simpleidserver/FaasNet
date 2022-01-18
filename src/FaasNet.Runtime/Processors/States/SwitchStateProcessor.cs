using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public class SwitchStateProcessor : BaseStateProcessor
    {
        public SwitchStateProcessor()
        {

        }

        public override WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Switch;

        protected override Task<StateProcessorResult> Handle(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var swichState = executionContext.StateDef as WorkflowDefinitionSwitchState;
            if (swichState.Conditions.Any(c => c.ConditionType == WorkflowDefinitionEventConditionTypes.DATA))
            {
                if (TryCheck(executionContext, swichState.Conditions.Cast<WorkflowDefinitionSwitchDataCondition>().ToList(), out WorkflowDefinitionSwitchDataCondition result))
                {
                    return Task.FromResult(Ok(result, executionContext.StateInstance.Input));
                }
            }
            else
            {
                if (TryCheck(executionContext, swichState.Conditions.Cast<WorkflowDefinitionSwitchEventCondition>().ToList(), out WorkflowDefinitionSwitchEventCondition result, out WorkflowInstanceStateEvent stateEvt))
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

        protected bool TryCheck(WorkflowInstanceExecutionContext executionContext, List<WorkflowDefinitionSwitchDataCondition> conditions, out WorkflowDefinitionSwitchDataCondition result)
        {
            result = null;
            foreach(var condition in conditions.Cast<WorkflowDefinitionSwitchDataCondition>())
            {
                if (executionContext.StateInstance.EvaluateCondition(condition.Condition))
                {
                    result = condition;
                    return true;
                }
            }

            return false;
        }

        protected bool TryCheck(WorkflowInstanceExecutionContext executionContext, List<WorkflowDefinitionSwitchEventCondition> conditions, out WorkflowDefinitionSwitchEventCondition result, out WorkflowInstanceStateEvent stateEvt)
        {
            result = null;
            stateEvt = null;
            foreach(var condition in conditions)
            {
                var evtRef = condition.EventRef;
                var evt = executionContext.WorkflowDef.GetEvent(evtRef);
                executionContext.Instance.TryListenEvent(executionContext.StateInstance.Id, evt.Name, evt.Source, evt.Type);
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
