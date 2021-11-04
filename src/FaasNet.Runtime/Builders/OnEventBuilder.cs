using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using System;

namespace FaasNet.Runtime.Builders
{
    public class OnEventBuilder
    {
        private readonly WorkflowDefinitionOnEvent _onEvent;

        public OnEventBuilder()
        {
            _onEvent = new WorkflowDefinitionOnEvent();
        }

        public OnEventBuilder AddEventRef(string evtRef)
        {
            _onEvent.EventRefs.Add(evtRef);
            return this;
        }

        public OnEventBuilder SetActionMode(WorkflowDefinitionActionModes actionMode)
        {
            _onEvent.ActionMode = actionMode;
            return this;
        }

        public OnEventBuilder SetDataFilter(string data, string toStateData, bool useData = true)
        {
            _onEvent.EventDataFilter = new WorkflowDefinitionEventDataFilter
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

        public WorkflowDefinitionOnEvent Build()
        {
            return _onEvent;
        }
    }
}
