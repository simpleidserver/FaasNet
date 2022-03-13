using FaasNet.Gateway.Core.StateMachineInstances.Results;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachineInstances.Queries
{
    public class GetStateMachineInstanceQuery : IRequest<StateMachineInstanceDetailsResult>
    {
        public string Id { get; set; }
    }
}
