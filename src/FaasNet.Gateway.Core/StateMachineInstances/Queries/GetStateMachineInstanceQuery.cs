using FaasNet.Gateway.Core.StateMachineInstances.Results;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachineInstances.Queries
{
    public class GetStateMachineInstanceQuery : IRequest<StateMachineInstanceResult>
    {
        public string Id { get; set; }
    }
}
