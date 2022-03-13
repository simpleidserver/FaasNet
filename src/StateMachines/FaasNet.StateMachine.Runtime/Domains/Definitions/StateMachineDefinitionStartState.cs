using FaasNet.StateMachine.Runtime.Domains.Enums;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionStartState
    {
        /// <summary>
        /// Name of the starting workflow state.
        /// </summary>
        public string StateName { get; set; }
        /// <summary>
        /// Defines the recurring time intervals or cron expressions at which workflow instances should be automatically started.
        /// </summary>
        public string Schedule { get; set; }
        public StateMachineDefinitionStartStateTypes Type { get; set; }
    }
}
