using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class OperationStateBuilder : BaseStateBuilder<StateMachineDefinitionOperationState>
    {
        internal OperationStateBuilder() : base(StateMachineDefinitionOperationState.Create())
        {
        }

        public OperationStateBuilder SetActionMode(StateMachineDefinitionActionModes actionMode)
        {
            StateDef.ActionMode = actionMode;
            return this;
        }

        public OperationStateBuilder AddAction(string name, Action<ActionBuilder> callback)
        {
            var builder = new ActionBuilder(name);
            callback(builder);
            StateDef.Actions.Add(builder.Build());
            return this;
        }

        public OperationStateBuilder End()
        {
            StateDef.End = true;
            return this;
        }
    }
}
