using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.UI.Stores.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientInfo
{
    [FeatureState]
    public record ClientInfoState
    {
        public ClientInfoState()
        {

        }

        public ClientViewModel Client { get; set; }
        public bool IsLoading { get; set; }

        public ClientInfoState(bool isLoading, ClientViewModel client)
        {
            IsLoading = isLoading;
            Client = client;
        }
    }
}
