using FaasNet.Gateway.Core.StateMachines.Queries.Results;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Commands
{
    public class StartStateMachineCommand : IRequest<StateMachineInstanceDetails>
    {
        public string Id { get; set; }
        public string Input { get; set; }
    }
}
