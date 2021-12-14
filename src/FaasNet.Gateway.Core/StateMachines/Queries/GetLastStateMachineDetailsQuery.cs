using FaasNet.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Queries
{
    public class GetLastStateMachineDetailsQuery : IRequest<WorkflowDefinitionAggregate>
    {
        public string Id { get; set; }
    }
}
