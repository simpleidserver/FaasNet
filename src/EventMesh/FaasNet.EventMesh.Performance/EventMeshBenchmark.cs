using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess;
using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Performance.Columns;
using FaasNet.EventMesh.Performance.Exporters;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FaasNet.EventMesh.Performance
{
    // https://openmessaging.cloud/docs/benchmarks/
    [Config(typeof(EventMeshBenchmarkConfig))]
    public class EventMeshBenchmark
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private const string messageTopic = "messageTopic";
        private const string vpn = "Vpn";
        private EventMeshClient _eventMeshClient;
        private EventMeshPublishSessionClient _pubSessionClient;
        private EventMeshSubscribeSessionClient _subSessionClient;
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
        private SemaphoreSlim _semStandardPartitionsLaunched = new SemaphoreSlim(0);
        private SemaphoreSlim _semClientPartitionsLaunched = new SemaphoreSlim(0);
        private StreamWriter _recordsFile;
        private Stopwatch sw = new Stopwatch();
        public int NbMessageSent { get; set; } = 0;
        public int NbMessageReceived { get; set; } = 1;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            var clusterNodes = new ConcurrentBag<ClusterPeer>();
            var firstNode = await NodeHelper.BuildAndStartNode(5000, clusterNodes, 30000, (p) =>
            {
                CheckPartitions(p);
            });
            var secondNode = await NodeHelper.BuildAndStartNode(5001, clusterNodes, 40000, (p) =>
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
            var addPubClient = await Retry(async () =>
            {
                var r = await _eventMeshClient.AddClient("pubClientId", vpn, new List<ClientPurposeTypes> { ClientPurposeTypes.PUBLISH });
                return (r, r.Status == null);
            });
            var addSubClient = await Retry(async () =>
            {
                var r = await _eventMeshClient.AddClient("subClientId", vpn, new List<ClientPurposeTypes> { ClientPurposeTypes.SUBSCRIBE });
                return (r, r.Status == null);
            });

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
            _pubSessionClient = await _eventMeshClient.CreatePubSession(addPubClient.ClientId, vpn, addPubClient.ClientSecret);
            _subSessionClient = await _eventMeshClient.CreateSubSession(addSubClient.ClientId, vpn, addSubClient.ClientSecret, addSubClient.ClientId);
#pragma warning disable 4014
            Task.Run(ListenMessages, _cancellationTokenSource.Token);
#pragma warning restore 4014
            if (File.Exists(Constants.RecordsFilePath)) File.Delete(Constants.RecordsFilePath);
            _recordsFile = new StreamWriter(Constants.RecordsFilePath);
            sw.Start();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _cancellationTokenSource.Cancel();
            _recordsFile.Dispose();
            sw.Stop();
        }

        [Benchmark]
        public async Task PublishAndReceiveMessage()
        {
            var cloudEvent = new CloudEvent
            {
                Type = "com.github.pull.create",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = $"Message number {NbMessageSent}",
                ["comexampleextension1"] = "value"
            };
            await Retry(async () =>
            {
                var r = await _pubSessionClient.PublishMessage( messageTopic, cloudEvent);
                return (r, r.Status == Client.Messages.PublishMessageStatus.SUCCESS);
            });
            NbMessageSent++;
            var record = $"{NbMessageSent}{Constants.Separator}{sw.ElapsedMilliseconds}{Constants.Separator}{NbMessageReceived}";
            File.WriteAllText(Constants.SummaryFilePath, record);
            _recordsFile.WriteLine(record);
        }

        private async Task<T> Retry<T>(Func<Task<(T, bool)>> callback, int nbRetry = 0) where T : class
        {
            var kvp = await callback();
            if(!kvp.Item2)
            {
                nbRetry++;
                if (nbRetry >= 10) return null;
                Thread.Sleep(200);
                return await Retry(callback, nbRetry);
            }

            return kvp.Item1;
        }

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

        private async void ListenMessages()
        {
            try
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var r = await _subSessionClient.ReadMessage(NbMessageReceived);
                    if (r.Status == Client.Messages.ReadMessageStatus.SUCCESS)
                        NbMessageReceived++;
                }
            }
            catch
            {

            }
        }
    }

    public class EventMeshBenchmarkConfig : ManualConfig
    {
        public EventMeshBenchmarkConfig()
        {
            AddJob(Job.Default
                // .WithToolchain(InProcessToolchain.Instance)
                .RunOncePerIteration()
                .WithIterationCount(100)
                .WithLaunchCount(1));
            AddColumn(new RequestSentColumn());
            AddColumn(new RequestReceivedColumn());
            AddDiagnoser(MemoryDiagnoser.Default);
            AddExporter(new CsvExporter(CsvSeparator.CurrentCulture));
            AddExporter(new HtmlExporter());
            AddExporter(new CsvMeasurementsWithRequestsExporter(CsvSeparator.CurrentCulture));
            WithOptions(ConfigOptions.DisableOptimizationsValidator);
        }
    }
}
