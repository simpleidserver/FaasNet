using FaasNet.Runtime.Domains.Definitions;

namespace FaasNet.Runtime.Builders
{
    public class SwitchStateBuilder : BaseStateBuilder<WorkflowDefinitionSwitchState>
    {
        internal SwitchStateBuilder() : base(WorkflowDefinitionSwitchState.Create())
        {
        }

        public SwitchStateBuilder AddDataCondition(string name, string transition, string condition, bool end = false)
        {
            StateDef.Conditions.Add(new WorkflowDefinitionSwitchDataCondition
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
            StateDef.Conditions.Add(new WorkflowDefinitionSwitchEventCondition
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
