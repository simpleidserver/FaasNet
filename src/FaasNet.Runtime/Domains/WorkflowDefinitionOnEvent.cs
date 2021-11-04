using FaasNet.Runtime.Domains.Enums;
using System.Collections.Generic;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowDefinitionOnEvent
    {
        public WorkflowDefinitionOnEvent()
        {
            EventRefs = new List<string>();
            ActionMode = WorkflowDefinitionActionModes.Sequential;
            Actions = new List<WorkflowDefinitionAction>();
        }

        /// <summary>
        /// References one or more unique event names in the defined workflow events.
        /// </summary>
        public ICollection<string> EventRefs { get; set; }
        /// <summary>
        /// Specifies how actions are to be performed (in sequence or in parallel). 
        /// Default is "sequential"
        /// </summary>
        public WorkflowDefinitionActionModes ActionMode { get; set; }
        /// <summary>
        /// Actions to be performed.
        /// </summary>
        public ICollection<WorkflowDefinitionAction> Actions { get; set; }
        /// <summary>
        /// Event data filter definition.
        /// </summary>
        public WorkflowDefinitionEventDataFilter EventDataFilter { get; set; }
    }
}
