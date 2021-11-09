using FaasNet.Runtime.Domains.Enums;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class BaseEventCondition
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type of condition.
        /// </summary>
        public WorkflowDefinitionEventConditionTypes ConditionType { get; set; }
        /// <summary>
        ///  Transition to another state.
        /// </summary>
        public string Transition { get; set; }
        /// <summary>
        /// End workflow.
        /// </summary>
        public bool End { get; set; }
    }
}
