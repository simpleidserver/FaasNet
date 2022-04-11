using FaasNet.Domain;

namespace FaasNet.StateMachineInstance.Persistence
{
    public class SearchWorkflowInstanceParameter : BaseSearchParameter
    {
        public string Vpn { get; set; }
    }
}
