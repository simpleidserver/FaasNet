using FaasNet.StateMachine.Core.Infrastructure;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core
{
    public class StateMachineDefLauncher : IStateMachineDefLauncher
    {
        private readonly IRuntimeEngine _runtimeEngine;
        private readonly IStateMachineInstanceRepository _stateMachineInstanceRepository;
        private readonly IIntegrationEventProcessor _integrationEventProcessor;

        public StateMachineDefLauncher(IRuntimeEngine runtimeEngine, IStateMachineInstanceRepository stateMachineInstanceRepository, IIntegrationEventProcessor integrationEventProcessor)
        {
            _runtimeEngine = runtimeEngine;
            _stateMachineInstanceRepository = stateMachineInstanceRepository;
            _integrationEventProcessor = integrationEventProcessor;
        }

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
            await _runtimeEngine.Launch(workflowDefinitionAggregate, workflowInstance, input, cancellationToken);
            await _stateMachineInstanceRepository.Add(workflowInstance, cancellationToken);
            await _stateMachineInstanceRepository.SaveChanges(cancellationToken);
            await Publish(workflowInstance, cancellationToken);
            return workflowInstance;
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

        private Task Publish(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            return _integrationEventProcessor.Process(workflowInstance.IntegrationEvents.ToList(), cancellationToken);
        }
    }
}
