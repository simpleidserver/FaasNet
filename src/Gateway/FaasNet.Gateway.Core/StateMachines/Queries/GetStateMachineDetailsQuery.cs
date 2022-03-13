using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Queries
{
    public class GetStateMachineDetailsQuery : IRequest<WorkflowDefinitionAggregate>
    {
        public string Id { get; set; }
    }
}
