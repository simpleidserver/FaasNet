using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class SwitchStateBuilder : BaseStateBuilder<StateMachineDefinitionSwitchState>
    {
        internal SwitchStateBuilder() : base(StateMachineDefinitionSwitchState.Create())
        {
        }

        public SwitchStateBuilder AddDataCondition(string name, string transition, string condition, bool end = false)
        {
            StateDef.Conditions.Add(new StateMachineDefinitionSwitchDataCondition
            {
                Condition = condition,
                Name = name,
                Transition = transition,
                End = end
            });
            return this;
        }

        public SwitchStateBuilder AddEventCondition(string name, string eventRef, string transition, bool end = false)
        {
            StateDef.Conditions.Add(new StateMachineDefinitionSwitchEventCondition
            {
                EventRef = eventRef,
                Name = name,
                End = end,
                Transition = transition
            });
            return this;
        }
    }
}
