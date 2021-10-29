using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Extensions;
using FaasNet.Runtime.Processors;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime
{
    public class RuntimeEngine
    {
        private readonly IEnumerable<IStateProcessor> _stateProcessors;

        public RuntimeEngine(IEnumerable<IStateProcessor> stateProcessors)
        {
            _stateProcessors = stateProcessors;
        }

        public Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken)
        {
            return InstanciateAndLaunch(workflowDefinitionAggregate, JObject.Parse(input), cancellationToken);
        }

        public async Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, JObject input, CancellationToken cancellationToken)
        {
            var workflowInstance = Instanciate(workflowDefinitionAggregate);
            await Launch(workflowDefinitionAggregate, workflowInstance, input, cancellationToken);
            return workflowInstance;
        }

        public async Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken)
        {
            WorkflowInstanceState state = null;
            if (!workflowInstance.TryGetFirstCreateState(out state))
            {
                return;
            }

            var stateDefinition = workflowDefinitionAggregate.GetState(state.DefId);
            workflowInstance.StartState(state.Id, Transform(input, stateDefinition.StateDataFilterInput));
            var stateProcessor = _stateProcessors.FirstOrDefault(s => s.Type == stateDefinition.Type);
            if (stateProcessor == null)
            {
                return;
            }

            var executionContext = new WorkflowInstanceExecutionContext(stateDefinition, state, workflowDefinitionAggregate);
            var stateProcessorResult = await stateProcessor.Process(executionContext, cancellationToken);
            if (stateProcessorResult.IsError)
            {
                return;
            }

            var output = Transform(stateProcessorResult.Output, stateDefinition.StateDataFilterOuput);
            workflowInstance.CompleteState(state.Id, output);
            if (stateDefinition.End)
            {
                workflowInstance.Terminate(output);
                return;
            }

            await Launch(workflowDefinitionAggregate, workflowInstance, output, cancellationToken);
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
    }
}
