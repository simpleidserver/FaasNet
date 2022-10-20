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

        public async Task<SearchSessionsResult> SearchSessions(string clientId, string vpn, FilterQuery filter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.SearchSessions(clientId, vpn, filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as SearchSessionsResult;
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

        public async Task<GetClientResult> GetClient(string clientId, string vpn, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetClient(clientId, vpn);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetClientResult;
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

        public async Task<AddQueueResponse> AddQueue(string vpn, string name, string topicFilter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddQueue(vpn, name, topicFilter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddQueueResponse;
        }

        public async Task<SearchQueuesResult> SearchQueues(FilterQuery filter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.SearchQueues(filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as SearchQueuesResult;
        }

        public async Task<FindVpnsByNameResult> FindVpnsByName(string name, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.FindVpnsByName(name);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as FindVpnsByNameResult;
        }

        public async Task<FindClientsByNameResult> FindClientsByName(string name, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.FindClientsByName(name);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as FindClientsByNameResult;
        }

        public async Task<FindQueuesByNameResult> FindQueuesByNames(string name, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.FindQueuesByName(name);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as FindQueuesByNameResult;
        }

        public async Task<GetPartitionResult> GetPartition(string partition, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetPartition(partition);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetPartitionResult;
        }

        public async Task<AddEventDefinitionResult> AddEventDefinition(string id, string vpn, string jsonSchema, string description, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddEventDefinition(id, vpn, jsonSchema, description);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddEventDefinitionResult;
        }

        public async Task<GetEventDefinitionResult> GetEventDefinition(string id, string vpn, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetEventDefinition(id, vpn);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetEventDefinitionResult;
        }

        public async Task<GetAllEventDefsResult> GetEventDefinitions(FilterQuery filter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetAllEventDefs(filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetAllEventDefsResult;
        }

        public async Task<UpdateEventDefinitionResult> UpdateEventDefinition(string id, string vpn, string jsonSchema, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.UpdateEventDefinition(id, vpn, jsonSchema);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as UpdateEventDefinitionResult;
        }

        public async Task<AddApplicationDomainResult> AddApplicationDomain(string name, string vpn, string description, string rootTopic, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddApplicationDomain(name, vpn, description, rootTopic);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddApplicationDomainResult;
        }

        public async Task<AddLinkApplicationDomainResult> AddApplicationDomainLink(string name, string vpn, string source, string target, string eventId, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddLinkApplicationDomain(name, vpn, source, target, eventId);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddLinkApplicationDomainResult;
        }

        public async Task<RemoveLinkApplicationDomainResult> RemoveApplicationDomainLink(string name, string vpn, string source, string target, string eventId, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.RemoveLinkApplicationDomain(name, vpn, source, target, eventId);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as RemoveLinkApplicationDomainResult;
        }

        public async Task<AddElementApplicationDomainResult> AddApplicationDomainElement(string name, string vpn, string elementId, double coordinateX, double coordinateY, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddApplicationDomainElement(name, vpn, elementId, coordinateX, coordinateY);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as AddElementApplicationDomainResult;
        }

        public async Task<RemoveElementApplicationDomainResult> RemoveApplicationDomainElement(string name, string vpn, string elementId, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.RemoveApplicationDomainElement(name, vpn, elementId);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as RemoveElementApplicationDomainResult;
        }

        public async Task<UpdateApplicationDomainCoordinatesResult> UpdateApplicationDomainCoordinates(string name, string vpn, ICollection<ApplicationDomainCoordinate> coordinates, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.UpdateApplicationDomainCoordinates(name, vpn, coordinates);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as UpdateApplicationDomainCoordinatesResult;
        }

        public async Task<GetApplicationDomainResult> GetApplicationDomain(string name, string vpn, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetApplicationDomain(name, vpn);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetApplicationDomainResult;
        }

        public async Task<GetAllApplicationDomainsResult> GetAllApplicationDomains(FilterQuery filter, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetAllApplicationDomains(filter);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = BaseEventMeshPackage.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetAllApplicationDomainsResult;
        }

        public async Task<EventMeshPublishSessionClient> CreatePubSession(string clientId, string vpn, string clientSecret, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.Hello(clientId, vpn, clientSecret, string.Empty, ClientPurposeTypes.PUBLISH);
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

        public async Task<EventMeshSubscribeSessionClient> CreateSubSession(string clientId, string vpn, string clientSecret, string queueName, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.Hello(clientId, vpn, clientSecret, queueName, ClientPurposeTypes.SUBSCRIBE);
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
