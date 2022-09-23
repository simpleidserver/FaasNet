using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Client
{
    public class EventMeshClient : BasePartitionedPeerClient
    {
        public EventMeshClient(IClientTransport transport) : base(transport) { }

        public async Task<PingResult> Ping(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
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
            return packageResult as PingResult;
        }

        public async Task<AddVpnResult> AddVpn(string vpn, string description = null, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddVpn(vpn, description);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddVpnResult;
        }

        public async Task<GetAllVpnResult> GetAllVpn(FilterQuery filter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetAllVpn(filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetAllVpnResult;
        }

        public async Task<AddClientResult> AddClient(string clientId, string vpn, ICollection<ClientPurposeTypes> purposes, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddClient(clientId, vpn, purposes);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddClientResult;
        }

        public async Task<GetClientQueryResult> GetClient(string clientId, int timeoutMS = 500, CancellationToken cancellationToken)
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetClient(filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetAllClientResult;
        }

        public async Task<GetAllClientResult> GetAllClient(FilterQuery filter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetAllClient(filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetAllClientResult;
        }

        public async Task<AddQueueResponse> AddQueue(string name, string topicFilter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddQueue(name, topicFilter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddQueueResponse;
        }

        public async Task<EventMeshPublishSessionClient> CreatePubSession(string clientId, string clientSecret, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.Hello(clientId, clientSecret, string.Empty, ClientPurposeTypes.PUBLISH);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            var result = new EventMeshPublishSessionClient(packageResult as HelloResult, Transport.CloneAndOpen());
            return result;
        }

        public async Task<EventMeshSubscribeSessionClient> CreateSubSession(string clientId, string clientSecret, string queueName, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.Hello(clientId, clientSecret, queueName, ClientPurposeTypes.SUBSCRIBE);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            var result = new EventMeshSubscribeSessionClient(packageResult as HelloResult, Transport.CloneAndOpen());
            return result;
        }

        internal static void EnsureSuccessStatus(BaseEventMeshPackage packageRequest, BaseEventMeshPackage packageResponse)
        {
            if (packageRequest.Seq != packageResponse.Seq) throw new EventMeshClientException("the seq in the request doesn't match the seq in the response");
        }
    }

    public class EventMeshPublishSessionClient : BasePartitionedPeerClient
    {
        private readonly HelloResult _session;

        public EventMeshPublishSessionClient(HelloResult session, IClientTransport transport) : base(transport)
        {
            _session = session;
        }

        public async Task<PublishMessageResult> PublishMessage(string topic, CloudEvent cloudEvt, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.PublishMessage(topic, _session.SessionId, cloudEvt);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as PublishMessageResult;
        }

        internal static void EnsureSuccessStatus(BaseEventMeshPackage packageRequest, BaseEventMeshPackage packageResponse)
        {
            if (packageRequest.Seq != packageResponse.Seq) throw new EventMeshClientException("the seq in the request doesn't match the seq in the response");
        }
    }

    public class EventMeshSubscribeSessionClient : BasePartitionedPeerClient
    {
        private readonly HelloResult _session;

        public EventMeshSubscribeSessionClient(HelloResult session, IClientTransport transport) : base(transport)
        {
            _session = session;
        }

        public async Task<ReadMessageResult> ReadMessage(int offset, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.ReadMessage(offset, _session.SessionId);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as ReadMessageResult;
        }

        internal static void EnsureSuccessStatus(BaseEventMeshPackage packageRequest, BaseEventMeshPackage packageResponse)
        {
            if (packageRequest.Seq != packageResponse.Seq) throw new EventMeshClientException("the seq in the request doesn't match the seq in the response");
        }
    }
}
