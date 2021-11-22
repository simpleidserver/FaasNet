using FaasNet.Gateway.Core.StateMachines.Results;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Commands
{
    public class StartStateMachineCommand : IRequest<StartStateMachineResult>
    {
        public string Id { get; set; }
        public string Input { get; set; }
    }
}
