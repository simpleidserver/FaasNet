using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public class InjectStateProcessor : BaseFlowableStateProcessor
    {
        public override StateMachineDefinitionStateTypes Type => StateMachineDefinitionStateTypes.Inject;

        protected override Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var injectState = executionContext.StateDef as StateMachineDefinitionInjectState;
            return Task.FromResult(Ok(injectState.Data, injectState));
        }
    }
}
