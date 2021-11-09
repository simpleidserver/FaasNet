using FaasNet.Runtime.Domains.Definitions;

namespace FaasNet.Runtime.Builders
{
    public interface IStateBuilder
    {
        BaseWorkflowDefinitionState Build();
        IStateBuilder SetOutputFilter(string filter);
        IStateBuilder SetName(string name);
    }
}
