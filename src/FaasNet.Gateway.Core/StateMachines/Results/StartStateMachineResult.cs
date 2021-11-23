using System;

namespace FaasNet.Gateway.Core.StateMachines.Results
{
    public class StartStateMachineResult
    {
        public string Id { get; set; }
        public DateTime LaunchDateTime { get; set; }
    }
}
