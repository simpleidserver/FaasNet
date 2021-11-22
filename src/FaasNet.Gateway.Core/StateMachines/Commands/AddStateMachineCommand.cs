using FaasNet.Gateway.Core.StateMachines.Results;
using FaasNet.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Commands
{
    public class AddStateMachineCommand : IRequest<AddStateMachineResult>
    {
        public WorkflowDefinitionAggregate WorkflowDefinition { get; set; }
    }
}
