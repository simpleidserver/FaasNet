using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Queue
{
    public class QueueConsumer : BaseIntegrationEventConsumer, IConsumer<ClientAdded>
    {
        private readonly PeerOptions _peerOptions;
        private readonly IPeerClientFactory _peerClientFactory;

        public QueueConsumer(IOptions<PeerOptions> peerOptions, IPeerClientFactory peerClientFactory, IPartitionCluster partitionCluster) : base(partitionCluster)
        {
            _peerOptions = peerOptions.Value;
            _peerClientFactory = peerClientFactory;
        }

        public async Task Consume(ClientAdded request, CancellationToken cancellationToken)
        {
            using (var eventMeshClient = _peerClientFactory.Build<EventMeshClient>(_peerOptions.Url, _peerOptions.Port))
            {
                await eventMeshClient.AddQueue(request.Vpn, request.ClientId, cancellationToken: cancellationToken);
            }
        }
    }
}
