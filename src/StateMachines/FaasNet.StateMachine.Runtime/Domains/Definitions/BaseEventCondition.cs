using FaasNet.StateMachine.Runtime.Domains.Enums;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public abstract class BaseEventCondition
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type of condition.
        /// </summary>
        public StateMachineDefinitionEventConditionTypes ConditionType { get; set; }
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
