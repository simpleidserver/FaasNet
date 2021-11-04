using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Extensions;
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
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public RuntimeEngine(IEnumerable<IStateProcessor> stateProcessors, ICloudEventSubscriptionRepository cloudEventSubscriptionRepository, IWorkflowInstanceRepository workflowInstanceRepository)
        {
            _stateProcessors = stateProcessors;
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
        }

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
            var rootState = workflowInstance.GetRootState();
            return Launch(workflowDefinitionAggregate, workflowInstance, input, rootState.Id, cancellationToken);
        }

        public async Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken)
        {
            await InternalLaunch(workflowDefinitionAggregate, workflowInstance, input, stateInstanceId, cancellationToken);
            await Publish(workflowInstance, cancellationToken);
        }

        protected async Task InternalLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken)
        {
            var stateInstance = workflowInstance.GetState(stateInstanceId);
            if (stateInstance.Status == Domains.Enums.WorkflowInstanceStateStatus.COMPLETE)
            {
                // TODO : THROW EXCEPTION.
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
                return;
            }

            var output = Transform(stateProcessorResult.Output, stateDefinition.StateDataFilterOuput);
            workflowInstance.CompleteState(stateInstance.Id, output);
            if (stateDefinition.End)
            {
                workflowInstance.Terminate(output);
                return;
            }

            var nextStateInstances = workflowInstance.GetNextStateInstanceIds(stateInstance.Id);
            foreach(var nextStateInstance in nextStateInstances)
            {
                await InternalLaunch(workflowDefinitionAggregate, workflowInstance, output, nextStateInstance, cancellationToken);
            }
        }

        public WorkflowInstanceAggregate Instanciate(WorkflowDefinitionAggregate workflowDefinitionAggregate)
        {
            var result = WorkflowInstanceAggregate.Create(workflowDefinitionAggregate.Id, workflowDefinitionAggregate.Version);
            var dic = new Dictionary<string, string>();
            foreach (var state in workflowDefinitionAggregate.States)
            {
                var stateInstance = result.AddState(state.Id);
                dic.Add(state.Id, stateInstance.Id);
            }

            foreach(var state in workflowDefinitionAggregate.States)
            {
                if (!string.IsNullOrWhiteSpace(state.Transition))
                {
                    result.AddFlow(dic[state.Id], dic[state.Transition]);
                }
            }

            return result;
        }

        private JObject Transform(JObject input, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return input;
            }

            return input.Transform(filter);
        }

        private async Task Publish(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            foreach (var evt in workflowInstance.EventAddedEvts)
            {
                await _cloudEventSubscriptionRepository.Add(CloudEventSubscriptionAggregate.Create(workflowInstance.Id, evt.StateInstanceId, evt.Source, evt.Type), cancellationToken);
                await _cloudEventSubscriptionRepository.SaveChanges(cancellationToken);
            }

            return;
        }
    }
}
