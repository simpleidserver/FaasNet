using FaasNet.StateMachine.Core.StateMachines.Results;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Commands
{
    public class UpdateStateMachineCommand : IRequest<AddStateMachineResult>
    {
        public string Id { get; set; }
        public StateMachineDefinitionAggregate WorkflowDefinition { get; set; }
    }
}
