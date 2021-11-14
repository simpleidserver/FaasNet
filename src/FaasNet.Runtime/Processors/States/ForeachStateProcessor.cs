using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public class ForeachStateProcessor : BaseFlowableStateProcessor, IStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public ForeachStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.ForEach;

        public async Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var foreachState = executionContext.StateDef as WorkflowDefinitionForeachState;
            if(string.IsNullOrWhiteSpace(foreachState.InputCollection))
            {
                return Ok(executionContext.StateInstance.Input, foreachState);
            }

            var tokens = executionContext.StateInstance.Input.SelectToken(foreachState.InputCollection) as JArray;
            var result = new JArray();
            Func<JToken, Task> callback = async (token) =>
            {
                 var record = await _actionExecutor.ExecuteAndConcatenate(
                    token,
                    WorkflowDefinitionActionModes.Parallel,
                    foreachState.Actions,
                    executionContext,
                    cancellationToken);
                result.Add(record);
            };
            if (foreachState.Mode == WorkflowDefinitionForeachStateModes.Sequential)
            {
                foreach (var token in tokens)
                {
                    await callback(token);
                }
            }
            else
            {
                var opt = new ParallelOptions();
                if (foreachState.BatchSize != null)
                {
                    opt.MaxDegreeOfParallelism = foreachState.BatchSize.Value;
                }

                Parallel.ForEach(
                    tokens,
                    opt,
                    async (token) =>
                    {
                        await callback(token);
                    });
            }

            return Ok(result, foreachState);
        }
    }
}