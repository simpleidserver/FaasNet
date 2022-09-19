using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using Microsoft.Extensions.Options;

namespace FaasNet.EventMesh.UI.Data
{
    public interface IEventMeshService
    {
        Task<bool> Ping(string url, int port, CancellationToken cancellationToken);
        Task<GetAllNodesResult> GetAllNodes(string url, int port, CancellationToken cancellationToken);
        Task<IEnumerable<(GetPeerStateResult, string)>> GetAllPeerStates(string url, int port, CancellationToken cancellationToken);
        Task<IEnumerable<ClientResult>> GetAllClients(string url, int port, CancellationToken cancellationToken);
        Task<IEnumerable<VpnResult>> GetAllVpns(string url, int port, CancellationToken cancellationToken);
    }

    public class EventMeshService : IEventMeshService
    {
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly EventMeshUIOptions _options;

        public EventMeshService(IPeerClientFactory peerClientFactory, IOptions<EventMeshUIOptions> options)
        {
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public async Task<bool> Ping(string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                try
                {
                    var result = await client.Ping(_options.RequestTimeoutMS, cancellationToken);
                    return result != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<GetAllNodesResult> GetAllNodes(string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<PartitionClient>(url, port))
            {
                var result = await client.GetAllNodes(_options.RequestTimeoutMS, cancellationToken);
                return result;
            }
        }

        public async Task<IEnumerable<(GetPeerStateResult, string)>> GetAllPeerStates(string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<RaftConsensusClient>(url, port))
            {
                client.ClientType = PartitionedPeerClientTypes.BROADCAST;
                var result = await client.GetPeerState(_options.RequestTimeoutMS, cancellationToken);
                return result;
            }
        }

        public async Task<IEnumerable<ClientResult>> GetAllClients(string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var clientResult = await client.GetAllClient(_options.RequestTimeoutMS, cancellationToken);
                return clientResult.Clients;
            }
        }

        public async Task<IEnumerable<VpnResult>> GetAllVpns(string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var vpnResult = await client.GetAllVpn(_options.RequestTimeoutMS, cancellationToken);
                return vpnResult.Vpns;
            }
        }
    }
}
