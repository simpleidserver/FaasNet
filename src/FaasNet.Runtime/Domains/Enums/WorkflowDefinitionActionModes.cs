using System.Runtime.Serialization;

namespace FaasNet.Runtime.Domains.Enums
{
    public enum WorkflowDefinitionActionModes
    {
        [EnumMember(Value = "sequential")]
        Sequential = 0,
        [EnumMember(Value = "parallel")]
        Parallel = 1
    }
}
