using FaasNet.EventStore;
using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Serializer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public class StateMachineLauncher : IStateMachineLauncher
    {
        private readonly IRuntimeEngine _runtimeEngine;
        private readonly ICommitAggregateHelper _commitAggregateHelper;
        private readonly IIntegrationEventProcessor _integrationEventProcessor;

        public StateMachineLauncher(IRuntimeEngine runtimeEngine, ICommitAggregateHelper commitAggregateHelper, IIntegrationEventProcessor integrationEventProcessor)
        {
            _runtimeEngine = runtimeEngine;
            _commitAggregateHelper = commitAggregateHelper;
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
            StateMachineRuntimeMeter.IncrementCreatedStateMachineInstance();
            using (var activity = StateMachineRuntimeMeter.StateMachineWorkerActivitySource.StartActivity("INSTANCIATE_AND_LAUNCH"))
            {
                var workflowInstance = Instanciate(workflowDefinitionAggregate);
                workflowInstance.Parameters = parameters;
                await _runtimeEngine.Launch(workflowDefinitionAggregate, workflowInstance, input, cancellationToken);
                await _integrationEventProcessor.Process(workflowInstance.IntegrationEvents.ToList(), cancellationToken);
                await _commitAggregateHelper.Commit(workflowInstance, cancellationToken);
                if (workflowInstance.Status == Runtime.Domains.Enums.StateMachineInstanceStatus.TERMINATE) StateMachineRuntimeMeter.IncrementTerminatedStateMachineInstance();
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                return workflowInstance;
            }
        }

        public async Task<StateMachineInstanceAggregate> Reactivate(string id, CancellationToken cancellationToken)
        {
            using (var activity = StateMachineRuntimeMeter.StateMachineWorkerActivitySource.StartActivity("REACTIVATE_INSTANCE"))
            {
                var runtimeSerializer = new RuntimeSerializer();
                var stateMachineInstance = await _commitAggregateHelper.Get<StateMachineInstanceAggregate>(id, cancellationToken);
                activity?.AddBaggage("StateMachineInstanceId", stateMachineInstance.Id);
                activity?.AddBaggage("StateMachineDefId", stateMachineInstance.WorkflowDefId);
                stateMachineInstance.Reactivate();
                var stateMachineDef = runtimeSerializer.DeserializeYaml(stateMachineInstance.SerializedDefinition);
                await _runtimeEngine.Launch(stateMachineDef, stateMachineInstance, new JObject(), cancellationToken);
                await _integrationEventProcessor.Process(stateMachineInstance.IntegrationEvents.ToList(), cancellationToken);
                await _commitAggregateHelper.Commit(stateMachineInstance, cancellationToken);
                if (stateMachineInstance.Status == Runtime.Domains.Enums.StateMachineInstanceStatus.TERMINATE) StateMachineRuntimeMeter.IncrementTerminatedStateMachineInstance();
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                return stateMachineInstance;
            }
        }

        public StateMachineInstanceAggregate Instanciate(StateMachineDefinitionAggregate workflowDefinitionAggregate)
        {
            var runtimeSerializer = new RuntimeSerializer();
            var result = StateMachineInstanceAggregate.Create(
                workflowDefinitionAggregate.TechnicalId,
                workflowDefinitionAggregate.Id,
                workflowDefinitionAggregate.Name,
                workflowDefinitionAggregate.Description,
                workflowDefinitionAggregate.Version,
                workflowDefinitionAggregate.Vpn,
                workflowDefinitionAggregate.RootTopic,
                runtimeSerializer.SerializeYaml(workflowDefinitionAggregate));
            var dic = new Dictionary<string, string>();
            foreach (var state in workflowDefinitionAggregate.States)
            {
                var stateInstance = result.AddState(state.Id);
                dic.Add(state.Id, stateInstance.Id);
            }

            return result;
        }
    }
}
