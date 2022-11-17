using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;

namespace FaasNet.EventMesh.Performance
{
    public class EventMeshClientHelper
    {
        private const int MAX_NB_RETRY = 10;
        private readonly EventMeshClient _client = null;

        private EventMeshClientHelper(string url, int port, IClientTransport transport)
        {
            _client = PeerClientFactory.Build<EventMeshClient>(url, port, transport);
        }

        public static EventMeshClientHelper CreateUDPClient(string url, int port) => new EventMeshClientHelper(url, port, new ClientUDPTransport());

        public Task AddVpn(string vpn) => Retry(async () =>
        {
            var r = await _client.AddVpn(vpn);
            return (r, r.Status == null);
        });

        public Task<AddQueueResponse> AddQueue(string queueName, string vpn) => Retry(async () =>
        {
            var r = await _client.AddQueue(vpn, queueName);
            return (r, r.Status == AddQueueStatus.SUCCESS);
        });

        public Task<AddSubscriptionResult> AddSubscription(string queueName, string topic, string vpn) => Retry(async () =>
        {
            var r = await _client.AddSubscription(queueName, topic, vpn);
            return (r, r.Status == AddSubscriptionStatus.OK);
        });

        public Task<AddClientResult> AddPubClient(string clientId, string vpn) => Retry(async () =>
        {
            var r = await _client.AddClient(clientId, vpn, new List<ClientPurposeTypes> { ClientPurposeTypes.PUBLISH });
            return (r, r.Success);
        });

        public Task<AddClientResult> AddSubClient(string clientId, string vpn) => Retry(async () =>
        {
            var r = await _client.AddClient(clientId, vpn, new List<ClientPurposeTypes> { ClientPurposeTypes.SUBSCRIBE });
            return (r, r.Success);
        });

        public Task<EventMeshPublishSessionClient> CreatePubSession(string clientId, string clientSecret, string vpn) => _client.CreatePubSession(clientId, vpn, clientSecret);

        public Task<EventMeshSubscribeSessionClient> CreateSubSession(string clientId, string clientSecret, string queueName, string vpn) => Retry(async () =>
        {
            try
            {
                var r = await _client.CreateSubSession(clientId, vpn, clientSecret, queueName);
                return (r, r.Session.Status == HelloMessageStatus.SUCCESS);
            }
            catch { return (null, false); }
        });

        private static async Task<T> Retry<T>(Func<Task<(T, bool)>> callback, int nbRetry = 0) where T : class
        {
            var kvp = await callback();
            if (!kvp.Item2)
            {
                nbRetry++;
                if (nbRetry >= MAX_NB_RETRY) return null;
                Thread.Sleep(200);
                return await Retry(callback, nbRetry);
            }

            return kvp.Item1;
        }
    }
}
