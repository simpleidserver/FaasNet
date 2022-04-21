using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Extensions;
using FaasNet.StateMachine.Runtime.Processors;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime
{
    public class RuntimeEngine : IRuntimeEngine
    {
        private readonly IEnumerable<IStateProcessor> _stateProcessors;

        public RuntimeEngine(IEnumerable<IStateProcessor> stateProcessors)
        {
            _stateProcessors = stateProcessors;
        }

        #region Public Methods

        public Task Launch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken)
        {
            var rootState = workflowDefinitionAggregate.GetRootState();
            var instance = workflowInstance.GetStateByDefId(rootState.Id);
            return Launch(workflowDefinitionAggregate, workflowInstance, input, instance.Id, cancellationToken);
        }

        public async Task Launch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken)
        {
            await InternalLaunch(workflowDefinitionAggregate, workflowInstance, input, stateInstanceId, cancellationToken);
        }

        #endregion

        protected async Task InternalLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JToken input, string stateInstanceId, CancellationToken cancellationToken)
        {
            if (workflowInstance.Status != Domains.Enums.StateMachineInstanceStatus.ACTIVE)
            {
                return;
            }

            var stateInstance = workflowInstance.GetState(stateInstanceId);
            var stateDefinition = workflowDefinitionAggregate.GetState(stateInstance.DefId);
            var stateProcessor = _stateProcessors.FirstOrDefault(s => s.Type == stateDefinition.Type);
            if (stateProcessor == null)
            {
                return;
            }

            if (stateInstance.Status == Domains.Enums.StateMachineInstanceStateStatus.CREATE)
            {
                workflowInstance.StartState(stateInstance.Id, Transform(input, stateDefinition.StateDataFilterInput));
            }

            JToken output = null;
            string transition = null;
            if (stateInstance.Status != Domains.Enums.StateMachineInstanceStateStatus.COMPLETE)
            {
                var executionStateResult = await ExecuteState(stateDefinition, stateInstance, workflowInstance, workflowDefinitionAggregate, stateProcessor, cancellationToken);
                if (!executionStateResult.ContinueExecution)
                {
                    return;
                }

                output = executionStateResult.Output;
                transition = executionStateResult.Transition;
            }
            else
            {
                output = stateInstance.GetOutput();
                transition = stateInstance.NextTransition;
            }

            var stateDef = workflowDefinitionAggregate.GetState(transition);
            var nextState = workflowInstance.GetStateByDefId(stateDef.Id);
            if (nextState != null)
            {
                await InternalLaunch(workflowDefinitionAggregate, workflowInstance, output, nextState.Id, cancellationToken);
            }
        }

        private async Task<ExecutionStateResult> ExecuteState(BaseStateMachineDefinitionState stateDefinition, StateMachineInstanceState stateInstance, StateMachineInstanceAggregate workflowInstance, StateMachineDefinitionAggregate workflowDefinitionAggregate, IStateProcessor stateProcessor, CancellationToken cancellationToken)
        {
            var executionContext = new StateMachineInstanceExecutionContext(stateDefinition, stateInstance, workflowInstance, workflowDefinitionAggregate);
            var stateProcessorResult = await stateProcessor.Process(executionContext, cancellationToken);
            if (stateProcessorResult.Status != StateProcessorStatus.OK)
            {
                switch (stateProcessorResult.Status)
                {
                    case StateProcessorStatus.BLOCKED:
                        workflowInstance.BlockState(stateInstance.Id);
                        break;
                    case StateProcessorStatus.ERROR:
                        workflowInstance.ErrorState(stateInstance.Id, stateProcessorResult.Exception);
                        break;
                }

                return ExecutionStateResult.Exit();
            }

            var output = Transform(stateProcessorResult.Output, stateDefinition.StateDataFilterOuput);
            workflowInstance.CompleteState(stateInstance.Id, output, stateProcessorResult.Transition);
            if (stateProcessorResult.IsEnd)
            {
                workflowInstance.Terminate(output);
                return ExecutionStateResult.Exit();
            }

            return ExecutionStateResult.Continue(output, stateProcessorResult.Transition);
        }

        private class ExecutionStateResult
        {
            public bool ContinueExecution { get; private set; }
            public JToken Output { get; private set; }
            public string Transition { get; private set; }

            public static ExecutionStateResult Exit()
            {
                return new ExecutionStateResult { ContinueExecution = false };
            }

            public static ExecutionStateResult Continue(JToken output, string transition)
            {
                return new ExecutionStateResult { ContinueExecution = true, Output = output, Transition = transition };
            }
        }

        private JToken Transform(JToken input, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return input;
            }

            return input.Transform(filter);
        }
    }
}
