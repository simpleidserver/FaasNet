using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public interface IStateBuilder
    {
        BaseStateMachineDefinitionState Build();
        IStateBuilder SetOutputFilter(string filter);
        IStateBuilder SetName(string name);
    }
}
