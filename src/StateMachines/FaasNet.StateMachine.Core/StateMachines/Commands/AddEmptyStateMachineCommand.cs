using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Commands
{
    public class AddEmptyStateMachineCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
