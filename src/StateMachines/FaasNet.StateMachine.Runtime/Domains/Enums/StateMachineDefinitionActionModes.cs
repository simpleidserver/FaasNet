using System.Runtime.Serialization;

namespace FaasNet.StateMachine.Runtime.Domains.Enums
{
    public enum StateMachineDefinitionActionModes
    {
        [EnumMember(Value = "sequential")]
        Sequential = 0,
        [EnumMember(Value = "parallel")]
        Parallel = 1
    }
}
