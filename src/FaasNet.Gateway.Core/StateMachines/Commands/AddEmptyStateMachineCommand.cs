using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Commands
{
    public class AddEmptyStateMachineCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
