using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.Extensions;
using FaasNet.StateMachine.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors
{
    public class ActionExecutor : IActionExecutor
    {
        private readonly IEnumerable<IFunctionProcessor> _functionProcessors;

        public ActionExecutor(IEnumerable<IFunctionProcessor> functionProcessors)
        {
            _functionProcessors = functionProcessors;
        }

        #region Public methods

        public async Task<JToken> ExecuteAndMerge(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var dic = await ExecuteActions(input, actionMode, actions, executionContext, cancellationToken);
            var output = input;
            foreach (var kvp in dic)
            {
                EnrichOutputState(kvp.Key, kvp.Value, output);
            }

            return output;
        }

        public async Task<JToken> ExecuteAndConcatenate(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var records = await Execute(input, actionMode, actions, executionContext, cancellationToken);
            JToken record = records.First();
            if (records.Count() > 1)
            {
                record = new JArray();
                foreach (var rec in records)
                {
                    ((JArray)record).Add(rec);
                }
            }

            return record;
        }

        public async Task<List<JToken>> Execute(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var dic = await ExecuteActions(input, actionMode, actions, executionContext, cancellationToken);
            return dic.Select(kvp => kvp.Value).ToList();
        }

        #endregion

        protected virtual async Task<Dictionary<StateMachineDefinitionAction, JToken>> ExecuteActions(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var result = new Dictionary<StateMachineDefinitionAction, JToken>();
            foreach (var action in actions)
            {
                var referenceName = action.FunctionRef.RefName;
                var function = executionContext.WorkflowDef.GetFunction(referenceName);
                if (function == null)
                {
                    throw new ProcessorException(string.Format(Global.UnknownWorkflowDefinition, referenceName));
                }

                var functionProcessor = _functionProcessors.FirstOrDefault(p => p.Type == function.Type);
                if (functionProcessor == null)
                {
                    throw new ProcessorException(string.Format(Global.UnknownFunctionProcessor, function.Type));
                }

                var functionInput = GetInput(action, input);
                var functionResult = await functionProcessor.Process(functionInput, function, executionContext.StateInstance, executionContext.Instance.Parameters, cancellationToken);
                result.Add(action, functionResult);
            }

            return result;
        }

        protected virtual JToken GetInput(StateMachineDefinitionAction action, JToken inputState)
        {
            if (action.ActionDataFilter != null && !string.IsNullOrWhiteSpace(action.ActionDataFilter.FromStateData))
            {
                inputState = inputState.Transform(action.ActionDataFilter.FromStateData);
            }

            if (!string.IsNullOrWhiteSpace(action.FunctionRef.ArgumentsStr))
            {
                return inputState.Transform(action.FunctionRef.ArgumentsStr);
            }

            return inputState;
        }

        protected virtual void EnrichOutputState(StateMachineDefinitionAction action, JToken functionResult, JToken outputState)
        {
            if (action.ActionDataFilter == null || (action.ActionDataFilter != null && action.ActionDataFilter.UseResults))
            {
                JToken inputToken = functionResult;
                if (!string.IsNullOrWhiteSpace(action.ActionDataFilter?.Results))
                {
                    inputToken = functionResult.Transform(action.ActionDataFilter?.Results);
                }

                if (inputToken == null)
                {
                    return;
                }

                outputState.Merge(action.ActionDataFilter?.ToStateData, inputToken);
            }
        }
    }
}
