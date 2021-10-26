using FaasNet.Runtime.Domains;

namespace FaasNet.Runtime.Builders
{
    public interface IStateBuilder
    {
        BaseWorkflowDefinitionState Build();
        IStateBuilder SetOutputFilter(string filter);
        IStateBuilder End();
    }
}
