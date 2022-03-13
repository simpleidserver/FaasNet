using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Extensions;
using FaasNet.StateMachine.Runtime.Infrastructure;
using FaasNet.StateMachine.Runtime.Persistence;
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
        private readonly IStateMachineInstanceRepository _workflowInstanceRepository;
        private readonly IIntegrationEventProcessor _integrationEventProcessor;

        public RuntimeEngine(IEnumerable<IStateProcessor> stateProcessors, IStateMachineInstanceRepository workflowInstanceRepository, IIntegrationEventProcessor integrationEventProcessor)
        {
            _stateProcessors = stateProcessors;
            _workflowInstanceRepository = workflowInstanceRepository;
            _integrationEventProcessor = integrationEventProcessor;
        }

        #region Public Methods

        public Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken)
        {
            return InstanciateAndLaunch(workflowDefinitionAggregate, input, new Dictionary<string, string>(), cancellationToken);
        }

        public Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, string input, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            return InstanciateAndLaunch(workflowDefinitionAggregate, JObject.Parse(input), parameters, cancellationToken);
        }

        public Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, JObject input, CancellationToken cancellationToken)
        {
            return InstanciateAndLaunch(workflowDefinitionAggregate, input, new Dictionary<string, string>(), cancellationToken);
        }

        public async Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, JObject input, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            var workflowInstance = Instanciate(workflowDefinitionAggregate);
            workflowInstance.Parameters = parameters;
            await Launch(workflowDefinitionAggregate, workflowInstance, input, cancellationToken);
            await _workflowInstanceRepository.Add(workflowInstance, cancellationToken);
            await _workflowInstanceRepository.SaveChanges(cancellationToken);
            return workflowInstance;
        }

        public Task Launch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken)
        {
            var rootState = workflowDefinitionAggregate.GetRootState();
            var instance = workflowInstance.GetStateByDefId(rootState.Id);
            return Launch(workflowDefinitionAggregate, workflowInstance, input, instance.Id, cancellationToken);
        }

        public async Task Launch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken)
        {
            await InternalLaunch(workflowDefinitionAggregate, workflowInstance, input, stateInstanceId, cancellationToken);
            await Publish(workflowInstance, cancellationToken);
        }

        #endregion

        protected async Task InternalLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JToken input, string stateInstanceId, CancellationToken cancellationToken)
        {
            var stateInstance = workflowInstance.GetState(stateInstanceId);
            if (stateInstance.Status == Domains.Enums.StateMachineInstanceStateStatus.COMPLETE)
            {
                return;
            }

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

                return;
            }

            var output = Transform(stateProcessorResult.Output, stateDefinition.StateDataFilterOuput);
            workflowInstance.CompleteState(stateInstance.Id, output);
            if (stateProcessorResult.IsEnd)
            {
                workflowInstance.Terminate(output);
                return;
            }

            var stateDef = workflowDefinitionAggregate.GetState(stateProcessorResult.Transition);
            var nextState = workflowInstance.GetStateByDefId(stateDef.Id);
            if (nextState != null)
            {
                await InternalLaunch(workflowDefinitionAggregate, workflowInstance, output, nextState.Id, cancellationToken);
            }
        }

        public StateMachineInstanceAggregate Instanciate(StateMachineDefinitionAggregate workflowDefinitionAggregate)
        {
            var result = StateMachineInstanceAggregate.Create(workflowDefinitionAggregate.TechnicalId, workflowDefinitionAggregate.Id, workflowDefinitionAggregate.Name, workflowDefinitionAggregate.Description, workflowDefinitionAggregate.Version);
            var dic = new Dictionary<string, string>();
            foreach (var state in workflowDefinitionAggregate.States)
            {
                var stateInstance = result.AddState(state.Id);
                dic.Add(state.Id, stateInstance.Id);
            }

            return result;
        }

        private JToken Transform(JToken input, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return input;
            }

            return input.Transform(filter);
        }

        private Task Publish(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            return _integrationEventProcessor.Process(workflowInstance.IntegrationEvents.ToList(), cancellationToken);
        }
    }
}
