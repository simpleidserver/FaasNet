using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Client
{
    public class EventMeshClient : BasePartitionedPeerClient
    {
        public EventMeshClient(IClientTransport transport) : base(transport) { }

        public async Task Ping(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.HeartBeat();
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
        }

        public async Task AddVpn(string vpn, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddVpn(vpn);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
        }

        internal static void EnsureSuccessStatus(BaseEventMeshPackage packageRequest, BaseEventMeshPackage packageResponse)
        {
            if (packageRequest.Seq != packageResponse.Seq) throw new EventMeshClientException("the seq in the request doesn't match the seq in the response");
        }
    }
}
