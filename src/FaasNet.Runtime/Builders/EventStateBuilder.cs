using FaasNet.Runtime.Domains;
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
    }
}
