using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public abstract class BaseStateBuilder<T> : IStateBuilder where T : BaseStateMachineDefinitionState
    {
        public BaseStateBuilder(T state)
        {
            StateDef = state;
        }

        protected T StateDef { get; private set; }

        public BaseStateMachineDefinitionState Build()
        {
            return StateDef;
        }

        public IStateBuilder SetName(string name)
        {
            StateDef.Id = name;
            StateDef.Name = name;
            return this;
        }

        public IStateBuilder SetInputFilter(string filter)
        {
            StateDef.StateDataFilterInput = filter;
            return this;
        }

        public IStateBuilder SetOutputFilter(string filter)
        {
            StateDef.StateDataFilterOuput = filter;
            return this;
        }
    }
}
