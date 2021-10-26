using FaasNet.Runtime.Domains.Enums;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowDefinitionStartState
    {
        /// <summary>
        /// Name of the starting workflow state.
        /// </summary>
        public string StateName { get; set; }
        /// <summary>
        /// Defines the recurring time intervals or cron expressions at which workflow instances should be automatically started.
        /// </summary>
        public string Schedule { get; set; }
        public WorkflowDefinitionStartStateTypes Type { get; set; }
    }
}
