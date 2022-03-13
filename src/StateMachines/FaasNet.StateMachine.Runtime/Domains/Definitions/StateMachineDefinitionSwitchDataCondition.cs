
namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionSwitchDataCondition : BaseEventCondition
    {
        public StateMachineDefinitionSwitchDataCondition()
        {
            ConditionType = Enums.StateMachineDefinitionEventConditionTypes.DATA;
        }

        /// <summary>
        /// Workflow expression evaluated against state data. Must evaluate to true or false.
        /// </summary>
        public string Condition { get; set; }
    }
}
