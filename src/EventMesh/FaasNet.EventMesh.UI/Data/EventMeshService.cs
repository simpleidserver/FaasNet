using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
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
        Task<GenericSearchQueryResult<ClientQueryResult>> GetAllClients(FilterQuery filter, string url, int port, CancellationToken cancellationToken);
        Task<GenericSearchQueryResult<VpnQueryResult>> GetAllVpns(FilterQuery filter, string url, int port, CancellationToken cancellationToken);
        Task<AddVpnResult> AddVpn(string vpn, string description, string url, int port, CancellationToken cancellationToken);
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

        public async Task<GenericSearchQueryResult<ClientQueryResult>> GetAllClients(FilterQuery filter, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var clientResult = await client.GetAllClient(filter, _options.RequestTimeoutMS, cancellationToken);
                return clientResult.Content;
            }
        }

        public async Task<GenericSearchQueryResult<VpnQueryResult>> GetAllVpns(FilterQuery filter, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var vpnResult = await client.GetAllVpn(filter, _options.RequestTimeoutMS, cancellationToken);
                return vpnResult.Content;
            }
        }

        public async Task<AddVpnResult> AddVpn(string vpn, string description, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var vpnResult = await client.AddVpn(vpn, description, _options.RequestTimeoutMS, cancellationToken);
                return vpnResult;
            }
        }
    }
}
