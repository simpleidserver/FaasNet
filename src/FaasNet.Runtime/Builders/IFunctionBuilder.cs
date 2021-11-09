using FaasNet.Runtime.Domains.Definitions;

namespace FaasNet.Runtime.Builders
{
    public interface IFunctionBuilder
    {
        WorkflowDefinitionFunction Build();
    }
}
