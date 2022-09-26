using FaasNet.EventMesh.Client.Messages;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientInfo
{
    [FeatureState]
    public class ClientInfoState
    {
        public ClientInfoState()
        {

        }

        public GetClientResult ClientInfo { get; set; }
        public bool IsLoading { get; set; }

        public ClientInfoState(bool isLoading, GetClientResult clientInfo)
        {
            IsLoading = isLoading;
            ClientInfo = clientInfo;
        }
    }
}
