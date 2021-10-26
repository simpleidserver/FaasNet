using FaasNet.Runtime.Domains.Enums;
using System.Collections.Generic;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowDefinitionOperationState : BaseWorkflowDefinitionState
    {
        /// <summary>
        /// Should actions be performed sequentially or in parallel.
        /// </summary>
        public WorkflowDefinitionOperationActionModes ActionMode { get; set; }
        /// <summary>
        /// Actions to be performed.
        /// </summary>
        public ICollection<WorkflowDefinitionAction> Actions { get; set; }
    }
}
