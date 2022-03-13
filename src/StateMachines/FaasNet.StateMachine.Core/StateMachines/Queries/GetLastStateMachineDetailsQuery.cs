using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Queries
{
    public class GetLastStateMachineDetailsQuery : IRequest<StateMachineDefinitionAggregate>
    {
        public string Id { get; set; }
    }
}
