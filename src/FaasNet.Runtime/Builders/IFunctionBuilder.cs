using FaasNet.Runtime.Domains;

namespace FaasNet.Runtime.Builders
{
    public interface IFunctionBuilder
    {
        WorkflowDefinitionFunction Build();
    }
}
