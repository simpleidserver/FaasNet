using FaasNet.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Commands
{
    public class UpdateStateMachineCommand : IRequest<bool>
    {
        public WorkflowDefinitionAggregate WorkflowDefinition { get; set; }
    }
}
