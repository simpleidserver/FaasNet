using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class OnEventBuilder
    {
        private readonly StateMachineDefinitionOnEvent _onEvent;

        public OnEventBuilder()
        {
            _onEvent = new StateMachineDefinitionOnEvent();
        }

        public OnEventBuilder AddEventRef(string evtRef)
        {
            _onEvent.EventRefs.Add(evtRef);
            return this;
        }

        public OnEventBuilder SetActionMode(StateMachineDefinitionActionModes actionMode)
        {
            _onEvent.ActionMode = actionMode;
            return this;
        }

        public OnEventBuilder SetDataFilter(string data, string toStateData, bool useData = true)
        {
            _onEvent.EventDataFilter = new StateMachineDefinitionEventDataFilter
            {
                Data = data,
                ToStateData = toStateData,
                UseData = useData
            };
            return this;
        }

        public OnEventBuilder AddAction(string name, Action<ActionBuilder> callback)
        {
            var builder = new ActionBuilder(name);
            callback(builder);
            _onEvent.Actions.Add(builder.Build());
            return this;
        }

        public StateMachineDefinitionOnEvent Build()
        {
            return _onEvent;
        }
    }
}
