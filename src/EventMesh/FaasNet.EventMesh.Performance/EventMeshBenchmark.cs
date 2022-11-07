using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;

namespace FaasNet.EventMesh.Performance
{
    // https://openmessaging.cloud/docs/benchmarks/
    [Config(typeof(EventMeshBenchmarkConfig))]
    public class EventMeshBenchmark
    {
        private const string messageTopic = "messageTopic";
        private EventMeshClient _eventMeshClient;
        private EventMeshPublishSessionClient _pubSessionClient;
        private EventMeshSubscribeSessionClient _subSessionClient;
        private int _messageOffset = 0;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            const string vpn = "Vpn";
            _eventMeshClient = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, new ClientUDPTransport());
            await _eventMeshClient.AddVpn(vpn);
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
        }

        [Benchmark]
        public async Task<int> PublishAndReadMessage()
        {
            var cloudEvent = new CloudEvent
            {
                Type = "com.github.pull.create",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = $"Message number {_messageOffset}",
                ["comexampleextension1"] = "value"
            };
            Console.WriteLine("Start publish");
            await Retry(async () =>
            {
                var r = await _pubSessionClient.PublishMessage(messageTopic, cloudEvent);
                return (r, r.Status == Client.Messages.PublishMessageStatus.SUCCESS);
            });
            Console.WriteLine("End publish");
            Console.WriteLine("Start read");
            var msg = await Retry(async () =>
            {
                var result = await _subSessionClient.ReadMessage(_messageOffset);
                return (result, result.Status == Client.Messages.ReadMessageStatus.SUCCESS);
            });
            Console.WriteLine("End read");
            _messageOffset++;
            return _messageOffset;
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
    }

    public class EventMeshBenchmarkConfig : ManualConfig
    {
        public EventMeshBenchmarkConfig()
        {
            AddJob(Job.Default.RunOncePerIteration()
                .WithIterationCount(20)
                .WithLaunchCount(1));
            AddDiagnoser(MemoryDiagnoser.Default);
            AddExporter(new CsvExporter(CsvSeparator.CurrentCulture));
            AddExporter(new HtmlExporter());
            WithOptions(ConfigOptions.DisableOptimizationsValidator);
        }
    }
}
