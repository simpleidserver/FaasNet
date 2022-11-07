using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

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

        public GenericSearchQueryResult<ClientLinkViewModel> Links
        {
            get
            {
                var result = new GenericSearchQueryResult<ClientLinkViewModel>();
                var records = new List<ClientLinkViewModel>();
                records.AddRange(Sources.Select(s => new ClientLinkViewModel
                {
                    ClientId = s.ClientId,
                    Direction = ClientLinkDirections.SUBSCRIBE,
                    EventId = s.EventId
                }));
                records.AddRange(Targets.Select(s => new ClientLinkViewModel
                {
                    ClientId = s.ClientId,
                    EventId = s.EventId,
                    Direction = ClientLinkDirections.PUBLISH
                }));
                result.Records = records;
                return result;
            }
        }
    }

    public class ClientLinkViewModel : ISerializable
    {
        public ClientLinkDirections Direction { get; set; }
        public string ClientId { get; set; }
        public string EventId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
        }

        public void Serialize(WriteBufferContext context)
        {
        }
    }

    public enum ClientLinkDirections
    {
        SUBSCRIBE = 0,
        PUBLISH = 1
    }
}
