using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public class InjectStateProcessor : BaseFlowableStateProcessor, IStateProcessor
    {
        public WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Inject;

        public Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var injectState = executionContext.StateDef as WorkflowDefinitionInjectState;
            return Task.FromResult(Ok(injectState.Data, injectState));
        }
    }
}
