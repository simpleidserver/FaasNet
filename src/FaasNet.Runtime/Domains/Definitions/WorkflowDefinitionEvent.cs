using FaasNet.Runtime.Domains.Enums;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionEvent
    {
        public WorkflowDefinitionEvent()
        {
            Kind = WorkflowDefinitionEventKinds.Consumed;
        }

        /// <summary>
        /// Unique event name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// CloudEvent source.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// CloudEvent type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Defines the event is either consumed or produced by the workflow. 
        /// Default is consumed
        /// </summary>
        public WorkflowDefinitionEventKinds Kind { get; set; }
    }
}
