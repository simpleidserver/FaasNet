using FaasNet.EventMesh.Client.StateMachines;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    [FeatureState]
    public record ClientState
    {
        public ClientState()
        {

        }

        public GenericSearchQueryResult<ClientViewModel> Clients { get; set; } = new GenericSearchQueryResult<ClientViewModel>();
        public bool IsLoading { get; set; }

        public ClientState(bool isLoading, GenericSearchQueryResult<ClientViewModel> clients)
        {
            IsLoading = isLoading;
            Clients = clients;
        }
    }
}
