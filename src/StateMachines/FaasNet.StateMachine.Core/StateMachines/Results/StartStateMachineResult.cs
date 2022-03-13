using System;

namespace FaasNet.StateMachine.Core.StateMachines.Results
{
    public class StartStateMachineResult
    {
        public string Id { get; set; }
        public DateTime LaunchDateTime { get; set; }
    }
}
