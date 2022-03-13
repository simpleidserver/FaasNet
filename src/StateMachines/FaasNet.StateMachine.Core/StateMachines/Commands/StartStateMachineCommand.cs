using FaasNet.StateMachine.Core.StateMachines.Results;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Commands
{
    public class StartStateMachineCommand : IRequest<StartStateMachineResult>
    {
        public string Id { get; set; }
        public string Input { get; set; }
        public string Parameters { get; set; }
    }
}
