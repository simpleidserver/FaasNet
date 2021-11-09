using FaasNet.Runtime.Domains.Definitions;

namespace FaasNet.Runtime.Builders
{
    public abstract class BaseStateBuilder<T> : IStateBuilder where T : BaseWorkflowDefinitionState
    {
        public BaseStateBuilder(T state)
        {
            StateDef = state;
        }

        protected T StateDef { get; private set; }

        public BaseWorkflowDefinitionState Build()
        {
            return StateDef;
        }

        public IStateBuilder SetName(string name)
        {
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
