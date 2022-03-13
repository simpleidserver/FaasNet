using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class ASYNCAPIFunctionDefinitionBuilder : IFunctionBuilder
    {
        private string _name;
        private string _operation;

        internal ASYNCAPIFunctionDefinitionBuilder(string name, string operation)
        {
            _name = name;
            _operation = operation;
        }

        public StateMachineDefinitionFunction Build()
        {
            return new StateMachineDefinitionFunction
            {
                Operation = _operation,
                Name = _name,
                Type = Domains.Enums.StateMachineDefinitionTypes.ASYNCAPI
            };
        }
    }
}
