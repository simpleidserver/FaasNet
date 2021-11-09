
namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionSwitchEventCondition : BaseEventCondition
    {
        public WorkflowDefinitionSwitchEventCondition()
        {
            ConditionType = Enums.WorkflowDefinitionEventConditionTypes.EVENT;
        }

        /// <summary>
        /// References an unique event name in the defined workflow events.
        /// </summary>
        public string EventRef { get; set; }
        /// <summary>
        /// Event Data Filter definition.
        /// </summary>
        public WorkflowDefinitionEventDataFilter EventDataFilter { get; set; }
    }
}
