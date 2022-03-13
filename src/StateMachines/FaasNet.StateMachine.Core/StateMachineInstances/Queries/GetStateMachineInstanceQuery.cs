using FaasNet.StateMachine.Core.StateMachineInstances.Results;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Queries
{
    public class GetStateMachineInstanceQuery : IRequest<StateMachineInstanceDetailsResult>
    {
        public string Id { get; set; }
    }
}
