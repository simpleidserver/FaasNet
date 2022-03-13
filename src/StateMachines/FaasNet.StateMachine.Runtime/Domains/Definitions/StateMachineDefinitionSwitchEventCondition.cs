
namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionSwitchEventCondition : BaseEventCondition
    {
        public StateMachineDefinitionSwitchEventCondition()
        {
            ConditionType = Enums.StateMachineDefinitionEventConditionTypes.EVENT;
        }

        /// <summary>
        /// References an unique event name in the defined workflow events.
        /// </summary>
        public string EventRef { get; set; }
        /// <summary>
        /// Event Data Filter definition.
        /// </summary>
        public virtual StateMachineDefinitionEventDataFilter EventDataFilter { get; set; }
    }
}
