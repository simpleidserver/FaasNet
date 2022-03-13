using Coeus;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public class ForeachStateProcessor : BaseFlowableStateProcessor
    {
        private readonly IActionExecutor _actionExecutor;

        public ForeachStateProcessor(IActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public override StateMachineDefinitionStateTypes Type => StateMachineDefinitionStateTypes.ForEach;

        protected override async Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var foreachState = executionContext.StateDef as StateMachineDefinitionForeachState;
            if(string.IsNullOrWhiteSpace(foreachState.InputCollection))
            {
                return Ok(executionContext.StateInstance.Input, foreachState);
            }

            var inputCollection = JTokenExtensions.CleanExpression(foreachState.InputCollection);
            var tokens = JQ.EvalToToken(inputCollection, executionContext.StateInstance.Input) as JArray;
            var result = new JArray();
            Func<JToken, Task> callback = async (token) =>
            {
                 var record = await _actionExecutor.ExecuteAndConcatenate(
                    token,
                    StateMachineDefinitionActionModes.Parallel,
                    foreachState.Actions,
                    executionContext,
                    cancellationToken);
                result.Add(record);
            };
            if (foreachState.Mode == StateMachineDefinitionForeachStateModes.Sequential)
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