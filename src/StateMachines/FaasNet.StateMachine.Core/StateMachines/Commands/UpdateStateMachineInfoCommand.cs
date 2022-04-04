using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Commands
{
    public class UpdateStateMachineInfoCommand : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
