using FaasNet.Common.Extensions;
using FaasNet.Discovery.Gossip.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Discovery.Gossip.Client
{
    public class GossipClient : BasePeerClient
    {
        public GossipClient(IClientTransport transport) : base(transport)
        {
        }

        public async Task Sync(ICollection<PeerInfo> peerInfos, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var package = GossipPackageRequestBuilder.Sync(peerInfos);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            await Receive().WithCancellation(cancellationToken);
        }

        public async Task<ICollection<PeerInfo>> Get(string partitionKey, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var package = GossipPackageRequestBuilder.Get(partitionKey);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = GossipPackage.Deserialize(readCtx, true);
            return (packageResult as GossipGetResultPackage).PeerInfos;

        }
    }
}
