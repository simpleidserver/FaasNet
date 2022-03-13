using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class ForeachStateBuilder :  BaseStateBuilder<StateMachineDefinitionForeachState>
    {
        internal ForeachStateBuilder() : base(StateMachineDefinitionForeachState.Create())
        {
        }


        public ForeachStateBuilder SetInputCollection(string inputCollection)
        {
            StateDef.InputCollection = inputCollection;
            return this;
        }

        public ForeachStateBuilder SetOutputCollection(string outputCollection)
        {
            StateDef.OutputCollection = outputCollection;
            return this;
        }

        public ForeachStateBuilder SetBatchSize(int batchSize)
        {
            StateDef.BatchSize = batchSize;
            return this;
        }

        public ForeachStateBuilder SetMode(StateMachineDefinitionForeachStateModes mode)
        {
            StateDef.Mode = mode;
            return this;
        }

        public ForeachStateBuilder AddAction(string name, Action<ActionBuilder> callback)
        {
            var builder = new ActionBuilder(name);
            callback(builder);
            StateDef.Actions.Add(builder.Build());
            return this;
        }

        public ForeachStateBuilder End()
        {
            StateDef.End = true;
            return this;
        }
    }
}
