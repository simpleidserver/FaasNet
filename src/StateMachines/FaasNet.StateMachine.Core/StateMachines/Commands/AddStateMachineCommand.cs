using FaasNet.StateMachine.Core.StateMachines.Results;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Commands
{
    public class AddStateMachineCommand : IRequest<AddStateMachineResult>
    {
        public StateMachineDefinitionAggregate WorkflowDefinition { get; set; }
    }
}
