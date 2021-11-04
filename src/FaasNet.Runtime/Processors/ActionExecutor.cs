using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.Extensions;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public class ActionExecutor : IActionExecutor
    {
        private readonly IEnumerable<IFunctionProcessor> _functionProcessors;

        public ActionExecutor(IEnumerable<IFunctionProcessor> functionProcessors)
        {
            _functionProcessors = functionProcessors;
        }

        public async Task<JObject> Execute(JObject input, WorkflowDefinitionActionModes actionMode, ICollection<WorkflowDefinitionAction> actions, WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            JObject result = input;
            foreach (var action in actions)
            {
                var referenceName = action.FunctionRef.ReferenceName;
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
                var functionResult = await functionProcessor.Process(functionInput, function, executionContext.StateInstance, cancellationToken);
                EnrichOutputState(action, functionResult, result);
            }

            return result;
        }

        protected virtual JObject GetInput(WorkflowDefinitionAction action, JObject inputState)
        {
            if (action.ActionDataFilter != null && !string.IsNullOrWhiteSpace(action.ActionDataFilter.FromStateData))
            {
                inputState = inputState.Transform(action.ActionDataFilter.ToStateData);
            }

            var input = JObject.Parse("{}");
            if (!string.IsNullOrWhiteSpace(action.FunctionRef.ArgumentsStr))
            {
                input = inputState.Transform(action.FunctionRef.ArgumentsStr);
            }

            return input;
        }

        protected virtual void EnrichOutputState(WorkflowDefinitionAction action, JObject functionResult, JObject outputState)
        {
            if (action.ActionDataFilter != null && action.ActionDataFilter.UseResults && !string.IsNullOrWhiteSpace(action.ActionDataFilter.ToStateData))
            {
                JToken inputToken = functionResult;
                if (!string.IsNullOrWhiteSpace(action.ActionDataFilter.Results))
                {
                    inputToken = functionResult.SelectToken(action.ActionDataFilter.Results);
                }

                if (inputToken == null)
                {
                    return;
                }

                outputState.Merge(action.ActionDataFilter.ToStateData, inputToken);
            }
        }
    }
}
