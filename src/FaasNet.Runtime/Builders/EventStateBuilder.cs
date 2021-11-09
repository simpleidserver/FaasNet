using FaasNet.Runtime.Domains.Definitions;
using System;

namespace FaasNet.Runtime.Builders
{
    public class EventStateBuilder : BaseStateBuilder<WorkflowDefinitionEventState>
    {
        public EventStateBuilder() : base(WorkflowDefinitionEventState.Create())
        {
        }

        public EventStateBuilder SetExclusive(bool exclusive)
        {
            StateDef.Exclusive = exclusive;
            return this;
        }

        public EventStateBuilder AddOnEvent(Action<OnEventBuilder> callback)
        {
            var builder = new OnEventBuilder();
            callback(builder);
            StateDef.OnEvents.Add(builder.Build());
            return this;
        }

        public EventStateBuilder End()
        {
            StateDef.End = true;
            return this;
        }
    }
}
