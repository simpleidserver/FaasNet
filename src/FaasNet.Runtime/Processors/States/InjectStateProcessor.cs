using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public class InjectStateProcessor : BaseFlowableStateProcessor
    {
        public override WorkflowDefinitionStateTypes Type => WorkflowDefinitionStateTypes.Inject;

        protected override Task<StateProcessorResult> Handle(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var injectState = executionContext.StateDef as WorkflowDefinitionInjectState;
            return Task.FromResult(Ok(injectState.Data, injectState));
        }
    }
}
