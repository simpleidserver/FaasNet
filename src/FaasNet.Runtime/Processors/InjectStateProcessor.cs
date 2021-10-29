using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public class InjectStateProcessor : IStateProcessor
    {
        public WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Inject;

        public Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var injectState = executionContext.StateDef as WorkflowDefinitionInjectState;
            return Task.FromResult(StateProcessorResult.Ok(injectState.Data));
        }
    }
}
