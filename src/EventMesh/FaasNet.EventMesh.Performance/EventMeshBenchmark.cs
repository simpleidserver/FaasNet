using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FaasNet.EventMesh.Performance
{
    // https://openmessaging.cloud/docs/benchmarks/
    public partial class EventMeshBenchmark
    {
        private const string messageTopic = "messageTopic";
        private const string vpn = "Vpn";
        private ConcurrentBag<ClusterPeer> clusterNodes = new ConcurrentBag<ClusterPeer>();
        private AddClientResult _addSubClient = null!;
        private AddClientResult _addPubClient = null!;
        private EventMeshClient _eventMeshClient = null!;
        private ConcurrentBag<string> _partitionKeys = new ConcurrentBag<string>();
        private List<string> _allStandardPartitionNames = new List<string>
        {
            PartitionNames.VPN_PARTITION_KEY,
            PartitionNames.CLIENT_PARTITION_KEY,
            PartitionNames.SESSION_PARTITION_KEY,
            PartitionNames.QUEUE_PARTITION_KEY,
            PartitionNames.EVENTDEFINITION_PARTITION_KEY,
            PartitionNames.APPLICATION_DOMAIN,
            PartitionNames.SUBSCRIPTION_PARTITION_KEY
        };
        private List<string> _allClientsPartitionNames = new List<string>
        {
            "Vpn_pubClientId",
            "Vpn_subClientId"
        };
        private CancellationTokenSource _subClientCancellationTokenSource = new CancellationTokenSource();
        private SemaphoreSlim _semStandardPartitionsLaunched = new SemaphoreSlim(0);
        private SemaphoreSlim _semClientPartitionsLaunched = new SemaphoreSlim(0);
        public int NbMessageSent { get; set; } = 0;
        public int NbMessageReceived { get; set; } = 1;

        public async Task Launch(int nbIterations = 100)
        {
            await GlobalSetup();
            await LaunchPublishAndReceiveMessage(nbIterations);
            Console.ReadLine();
            await LaunchNewNodeAndReceiveMessage(nbIterations);
            GlobalCleanup();
        }

        private async Task ExecuteOperation(Func<Task> callback, int nbIterations = 100)
        {
            var sw = new Stopwatch();
            for (var i = 0; i < nbIterations; i++)
            {
                sw.Reset();
                sw.Start();
                await callback();
                sw.Stop();
                Console.WriteLine($"operation {i} is executed in {sw.ElapsedMilliseconds}MS");
            }
        }

        #region Setup and cleanup

        private async Task GlobalSetup()
        {
            await NodeHelper.BuildAndStartNode(5000, clusterNodes, 30000, (p) =>
            {
                CheckPartitions(p);
            });
            await NodeHelper.BuildAndStartNode(5001, clusterNodes, 40000, (p) =>
            {
                CheckPartitions(p);
            });
            _semStandardPartitionsLaunched.Wait();
            Console.WriteLine("Standard partitions are launched");

            _eventMeshClient = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, new ClientUDPTransport());
            await Retry(async () =>
            {
                var r = await _eventMeshClient.AddVpn(vpn);
                return (r, r.Status == null);
            });
            _addPubClient = await Retry(async () =>
            {
                var r = await _eventMeshClient.AddClient("pubClientId", vpn, new List<ClientPurposeTypes> { ClientPurposeTypes.PUBLISH });
                return (r, r.Status == null);
            });
            _addSubClient = await Retry(async () =>
            {
                var r = await _eventMeshClient.AddClient("subClientId", vpn, new List<ClientPurposeTypes> { ClientPurposeTypes.SUBSCRIBE });
                return (r, r.Status == null);
            });
            await _eventMeshClient.AddQueue(vpn, "QueueName");

            _semClientPartitionsLaunched.Wait();
            Console.WriteLine("Clients partitions are launched");

            await Retry(async () =>
            {
                var r = await _eventMeshClient.AddApplicationDomain("appdomain", vpn, "description", "rootTopic");
                return (r, r.Status == Client.Messages.AddApplicationDomainStatus.OK);
            });
            await Retry(async () =>
            {
                var r = await _eventMeshClient.AddEventDefinition("evtId", vpn, "{}", "description", messageTopic);
                return (r, r.Status == Client.Messages.AddEventDefinitionStatus.OK);
            });
            await Retry(async () =>
            {
                var r = await _eventMeshClient.AddApplicationDomainElement("appdomain", vpn, "pubClientId", 0, 0, new List<ApplicationDomainElementPurposeTypes>
                {
                    ApplicationDomainElementPurposeTypes.PUBLISH
                });
                return (r, r.Status == Client.Messages.AddElementApplicationDomainStatus.OK);
            });
            await Retry(async () =>
            {
                var r = await _eventMeshClient.AddApplicationDomainElement("appdomain", vpn, "subClientId", 0, 0, new List<ApplicationDomainElementPurposeTypes>
                {
                    ApplicationDomainElementPurposeTypes.SUBSCRIBE
                });
                return (r, r.Status == Client.Messages.AddElementApplicationDomainStatus.OK);
            });
            await Retry(async () =>
            {
                var r = await _eventMeshClient.AddApplicationDomainLink("appdomain", vpn, "pubClientId", "subClientId", "evtId");
                return (r, r.Status == Client.Messages.AddLinkApplicationDomainStatus.OK);
            });
        }

        private void GlobalCleanup() { }

        private void CheckPartitions(string partitionKey)
        {
            Console.WriteLine($"Partition is launched {partitionKey}");
            if (_partitionKeys.Contains(partitionKey)) return;
            _partitionKeys.Add(partitionKey);
            var isStandardPartitionsLaunched = _allStandardPartitionNames.All(n => _partitionKeys.Contains(n));
            if(isStandardPartitionsLaunched)
                _semStandardPartitionsLaunched.Release();
            var isClientPartitionsLaunched = _allClientsPartitionNames.All(n => _partitionKeys.Contains(n));
            if (isClientPartitionsLaunched)
                _semClientPartitionsLaunched.Release();
        }

        private static async Task<T> Retry<T>(Func<Task<(T, bool)>> callback, int nbRetry = 0) where T : class
        {
            var kvp = await callback();
            if (!kvp.Item2)
            {
                nbRetry++;
                if (nbRetry >= 10) return null;
                Thread.Sleep(200);
                return await Retry(callback, nbRetry);
            }

            return kvp.Item1;
        }

        #endregion
    }
}
