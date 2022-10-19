using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ApplicationDomains
{
    [FeatureState]
    public record ApplicationDomainState
    {
        public ApplicationDomainState()
        {

        }

        public bool IsLoading { get; set; }
        public GenericSearchQueryResult<ApplicationDomainQueryResult> ApplicationDomains { get; set; } = new GenericSearchQueryResult<ApplicationDomainQueryResult>();
        public ApplicationDomainQueryResult ApplicationDomain { get; set; } = null!;

        public ApplicationDomainState(bool isLoading, GenericSearchQueryResult<ApplicationDomainQueryResult> applicationDomains)
        {
            IsLoading = isLoading;
            ApplicationDomains = applicationDomains;
        }
    }
}
