using FaasNet.Runtime.Domains;

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

        public IStateBuilder End()
        {
            StateDef.End = true;
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
