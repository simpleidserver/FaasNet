using FaasNet.Gateway.Core.StateMachines.Results;
using FaasNet.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Commands
{
    public class UpdateStateMachineCommand : IRequest<AddStateMachineResult>
    {
        public string Id { get; set; }
        public WorkflowDefinitionAggregate WorkflowDefinition { get; set; }
    }
}
