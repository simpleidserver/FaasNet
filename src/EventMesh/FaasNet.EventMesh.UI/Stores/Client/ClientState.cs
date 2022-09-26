using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    [FeatureState]
    public class ClientState
    {
        public ClientState()
        {

        }

        public GenericSearchQueryResult<ClientQueryResult> Clients { get; set; } = new GenericSearchQueryResult<ClientQueryResult>();
        public bool IsLoading { get; set; }

        public ClientState(bool isLoading, GenericSearchQueryResult<ClientQueryResult> clients)
        {
            IsLoading = isLoading;
            Clients = clients;
        }
    }
}
