using FaasNet.EventMesh.Client.StateMachines.Client;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public class ClientViewModel : ClientQueryResult
    {
        public ClientViewModel() { }

        public ClientViewModel(ClientQueryResult result)
        {
            Id = result.Id;
            ClientSecret = result.ClientSecret;
            Vpn = result.Vpn;
            SessionExpirationTimeMS = result.SessionExpirationTimeMS;
            Purposes = result.Purposes;
            CreateDateTime = result.CreateDateTime;
            Sources = result.Sources;
            Targets = result.Targets;
        }

        public bool IsSelected { get; set; }
    }
}
