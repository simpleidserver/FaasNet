using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.Client.StateMachines.Session;
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
        Task<GenericSearchQueryResult<SessionQueryResult>> SearchSessions(string clientId, string vpn, FilterQuery filter, string url, int port, CancellationToken cancellationToken);
        Task<GenericSearchQueryResult<QueueQueryResult>> SearchQueues(FilterQuery filter, string url, int port, CancellationToken cancellationToken);
        Task<AddVpnResult> AddVpn(string vpn, string description, string url, int port, CancellationToken cancellationToken);
        Task<AddClientResult> AddClient(string clientId, string vpn, ICollection<ClientPurposeTypes> purposeTypes, string url, int port, CancellationToken cancellationToken, double coordinateX = default(double), double coordinateY = default(double));
        Task<BulkUpdateClientResult> BulkUpdateClient(string vpn, ICollection<UpdateClientRequest> clients, string url, int port, CancellationToken cancellationToken);
        Task<GetClientResult> GetClient(string clientId, string vpn, string url, int port, CancellationToken cancellationToken);
        Task<AddQueueResponse> AddQueue(string vpn, string name, string topicFilter, string url, int port, CancellationToken cancellationToken);
        Task<PublishMessageResult> PublishMessage(string clientId, string vpn, string clientSecret, string topicMessage, string content, string url, int port, CancellationToken cancellationToken);
        Task<SubscriptionResult> Subscribe(string clientId, string vpn, string clientSecret, string queueName, string url, int port, CancellationToken cancellationToken);
        Task<IEnumerable<string>> FindVpnsByName(string name, string url, int port, CancellationToken cancellationToken);
        Task<IEnumerable<string>> FindClientsByName(string name, string url, int port, CancellationToken cancellationToken);
        Task<IEnumerable<string>> FindQueuesByName(string name, string url, int port, CancellationToken cancellationToken);
        Task<GetPartitionResult> GetPartition(string partition, string url, int port, CancellationToken cancellationToken);
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

        public async Task<GenericSearchQueryResult<SessionQueryResult>> SearchSessions(string clientId, string vpn, FilterQuery filter, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var sessionsResult = await client.SearchSessions(clientId, vpn, filter, _options.RequestTimeoutMS, cancellationToken);
                return sessionsResult.Content;
            }
        }

        public async Task<GenericSearchQueryResult<QueueQueryResult>> SearchQueues(FilterQuery filter, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var queuesResult = await client.SearchQueues(filter, _options.RequestTimeoutMS, cancellationToken);
                return queuesResult.Content;
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

        public async Task<AddClientResult> AddClient(string clientId, string vpn, ICollection<ClientPurposeTypes> purposeTypes, string url, int port, CancellationToken cancellationToken, double coordinateX = default(double), double coordinateY = default(double))
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var clientResult = await client.AddClient(clientId, vpn, purposeTypes, coordinateY, coordinateY, _options.RequestTimeoutMS, cancellationToken);
                return clientResult;
            }
        }

        public async Task<BulkUpdateClientResult> BulkUpdateClient(string vpn, ICollection<UpdateClientRequest> clients, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var clientResult = await client.UpdateClient(vpn, clients, _options.RequestTimeoutMS, cancellationToken);
                return clientResult;
            }
        }

        public async Task<GetClientResult> GetClient(string clientId, string vpn, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var clientResult = await client.GetClient(clientId, vpn, _options.RequestTimeoutMS, cancellationToken);
                return clientResult;
            }
        }

        public async Task<AddQueueResponse> AddQueue(string vpn, string name, string topicFilter, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var result = await client.AddQueue(vpn, name, topicFilter, _options.RequestTimeoutMS, cancellationToken);
                return result;
            }
        }

        public async Task<PublishMessageResult> PublishMessage(string clientId, string vpn, string clientSecret, string topicMessage, string content, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var pubSession = await client.CreatePubSession(clientId, vpn, clientSecret, _options.RequestTimeoutMS, cancellationToken);
                var cloudEvent = new CloudEvent
                {
                    Type = "com.github.pull.create",
                    Source = new Uri("https://github.com/cloudevents/spec/pull"),
                    Subject = "123",
                    Id = "A234-1234-1234",
                    Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                    DataContentType = "application/json",
                    Data = content,
                    ["comexampleextension1"] = "value"
                };
                return await Retry(async () =>
                {
                    var result = await pubSession.PublishMessage(topicMessage, cloudEvent, _options.RequestTimeoutMS, cancellationToken);
                    return (result, result.Status == PublishMessageStatus.SUCCESS);
                });
            }
        }

        public async Task<GetPartitionResult> GetPartition(string partition, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                return await client.GetPartition(partition, _options.RequestTimeoutMS, cancellationToken);
            }
        }

        public async Task<IEnumerable<string>> FindVpnsByName(string name, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var findResult = await client.FindVpnsByName(name, _options.RequestTimeoutMS, cancellationToken);
                return findResult.Content;
            }
        }

        public async Task<IEnumerable<string>> FindClientsByName(string name, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var findResult = await client.FindClientsByName(name, _options.RequestTimeoutMS, cancellationToken);
                return findResult.Content;
            }
        }

        public async Task<IEnumerable<string>> FindQueuesByName(string name, string url, int port, CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(url, port))
            {
                var findResult = await client.FindQueuesByNames(name, _options.RequestTimeoutMS, cancellationToken);
                return findResult.Content;
            }
        }

        public async Task<SubscriptionResult> Subscribe(string clientId, string vpn, string clientSecret, string queueName, string url, int port, CancellationToken cancellationToken)
        {
            var result = new SubscriptionResult(_peerClientFactory, url, port, _options.RequestTimeoutMS);
            await result.Init(clientId, vpn, clientSecret, queueName, cancellationToken);
            return result;
        }

        private async Task<T> Retry<T>(Func<Task<(T, bool)>> callback, int nbRetry = 0)
        {
            var result = await callback();
            if (!result.Item2)
            {
                if (nbRetry >= 5) return result.Item1;
                Thread.Sleep(500);
                nbRetry++;
                return await Retry(callback, nbRetry);
            }

            return result.Item1;
        }
    }

    public class SubscriptionResult : IDisposable
    {
        private readonly EventMeshClient _client;
        private readonly int _timeoutMS;
        private EventMeshSubscribeSessionClient _pubSession;

        public SubscriptionResult(IPeerClientFactory peerClientFactory, string url, int port, int timeoutMS)
        {
            _timeoutMS = timeoutMS;
            _client = peerClientFactory.Build<EventMeshClient>(url, port);
        }

        public async Task Init(string clientId, string vpn, string clientSecret, string queueName, CancellationToken cancellationToken)
        {
            _pubSession = await _client.CreateSubSession(clientId, vpn, clientSecret, queueName, _timeoutMS, cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public Task<ReadMessageResult> ReadMessage(int offset)
        {
            return _pubSession.ReadMessage(offset, _timeoutMS, CancellationToken.None);
        }
    }
}
