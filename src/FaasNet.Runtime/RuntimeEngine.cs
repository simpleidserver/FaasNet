using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Extensions;
using FaasNet.Runtime.Infrastructure;
using FaasNet.Runtime.Persistence;
using FaasNet.Runtime.Processors;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime
{
    public class RuntimeEngine : IRuntimeEngine
    {
        private readonly IEnumerable<IStateProcessor> _stateProcessors;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IIntegrationEventProcessor _integrationEventProcessor;

        public RuntimeEngine(IEnumerable<IStateProcessor> stateProcessors, IWorkflowInstanceRepository workflowInstanceRepository, IIntegrationEventProcessor integrationEventProcessor)
        {
            _stateProcessors = stateProcessors;
            _workflowInstanceRepository = workflowInstanceRepository;
            _integrationEventProcessor = integrationEventProcessor;
        }

        #region Public Methods

        public Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken)
        {
            return InstanciateAndLaunch(workflowDefinitionAggregate, JObject.Parse(input), cancellationToken);
        }

        public async Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, JObject input, CancellationToken cancellationToken)
        {
            var workflowInstance = Instanciate(workflowDefinitionAggregate);
            await Launch(workflowDefinitionAggregate, workflowInstance, input, cancellationToken);
            await _workflowInstanceRepository.Add(workflowInstance, cancellationToken);
            await _workflowInstanceRepository.SaveChanges(cancellationToken);
            return workflowInstance;
        }

        public Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken)
        {
            var rootState = workflowDefinitionAggregate.GetRootState();
            var instance = workflowInstance.GetStateByDefId(rootState.Id);
            return Launch(workflowDefinitionAggregate, workflowInstance, input, instance.Id, cancellationToken);
        }

        public async Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken)
        {
            await InternalLaunch(workflowDefinitionAggregate, workflowInstance, input, stateInstanceId, cancellationToken);
            await Publish(workflowInstance, cancellationToken);
        }

        #endregion

        protected async Task InternalLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JToken input, string stateInstanceId, CancellationToken cancellationToken)
        {
            var stateInstance = workflowInstance.GetState(stateInstanceId);
            if (stateInstance.Status == Domains.Enums.WorkflowInstanceStateStatus.COMPLETE)
            {
                return;
            }

            var stateDefinition = workflowDefinitionAggregate.GetState(stateInstance.DefId);
            var stateProcessor = _stateProcessors.FirstOrDefault(s => s.Type == stateDefinition.Type);
            if (stateProcessor == null)
            {
                return;
            }

            if (stateInstance.Status == Domains.Enums.WorkflowInstanceStateStatus.CREATE)
            {
                workflowInstance.StartState(stateInstance.Id, Transform(input, stateDefinition.StateDataFilterInput));
            }

            var executionContext = new WorkflowInstanceExecutionContext(stateDefinition, stateInstance, workflowInstance, workflowDefinitionAggregate);
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

        public WorkflowInstanceAggregate Instanciate(WorkflowDefinitionAggregate workflowDefinitionAggregate)
        {
            var result = WorkflowInstanceAggregate.Create(workflowDefinitionAggregate.TechnicalId, workflowDefinitionAggregate.Id, workflowDefinitionAggregate.Name, workflowDefinitionAggregate.Description, workflowDefinitionAggregate.Version);
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

        private Task Publish(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            return _integrationEventProcessor.Process(workflowInstance.IntegrationEvents.ToList(), cancellationToken);
        }
    }
}
