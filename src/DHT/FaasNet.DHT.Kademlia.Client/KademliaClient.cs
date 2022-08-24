using FaasNet.Common.Extensions;
using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Client
{
    public class KademliaClient : BasePeerClient
    {
        public KademliaClient(IClientTransport transport) : base(transport) { }

        public async Task<PingResult> Ping(CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.Ping(nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = KademliaPackage.Deserialize(readCtx);
            return packageResult as PingResult;
        }

        public async Task<FindValueResult> FindValue(long key, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.FindValue(key, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = KademliaPackage.Deserialize(readCtx);
            return packageResult as FindValueResult;
        }

        public async Task<FindNodeResult> FindNode(long key, string url, int port, long targetId, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.FindNode(key, url, port, targetId, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = KademliaPackage.Deserialize(readCtx);
            return packageResult as FindNodeResult;
        }

        public async Task<StoreResult> StoreValue(long key, string value, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.StoreValue(key, value, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = KademliaPackage.Deserialize(readCtx);
            return packageResult as StoreResult;
        }

        public async Task<StoreResult> ForceStoreValue(long key, string value, long excludedPeer, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.ForceStoreValue(key, value, nonce, excludedPeer);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = KademliaPackage.Deserialize(readCtx);
            return packageResult as StoreResult;
        }
    }
}
