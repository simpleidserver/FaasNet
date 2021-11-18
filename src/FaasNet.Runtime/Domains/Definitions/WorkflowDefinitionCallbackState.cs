using FaasNet.Runtime.Domains.Enums;
using System;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionCallbackState : BaseWorkflowDefinitionFlowableState
    {
        public WorkflowDefinitionCallbackState()
        {
            Type = WorkflowDefinitionStateTypes.Callback;
        }

        /// <summary>
        /// Defines the action to be executed.
        /// </summary>
        public virtual WorkflowDefinitionAction Action { get; set; }
        /// <summary>
        /// References an unique callback event name in the defined workflow events.
        /// </summary>
        public string EventRef { get; set; }
        public int? ActionId { get; set; }

        public static WorkflowDefinitionCallbackState Create()
        {
            return new WorkflowDefinitionCallbackState
            {
                Id = Guid.NewGuid().ToString(),
                Type = WorkflowDefinitionStateTypes.Callback
            };
        }
    }
}
