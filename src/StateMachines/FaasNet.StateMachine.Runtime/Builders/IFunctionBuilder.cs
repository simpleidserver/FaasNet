using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public interface IFunctionBuilder
    {
        StateMachineDefinitionFunction Build();
    }
}
