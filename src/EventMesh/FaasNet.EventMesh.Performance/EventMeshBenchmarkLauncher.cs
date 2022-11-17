namespace FaasNet.EventMesh.Performance
{
    // https://openmessaging.cloud/docs/benchmarks/
    public class EventMeshBenchmarkLauncher
    {
        private readonly EventMeshPartitionedPeerPool _serverPool = EventMeshPartitionedPeerPool.Create();
        private readonly EventMeshClientHelper _client = EventMeshClientHelper.CreateUDPClient(BenchmarkGlobalContext.Url, BenchmarkGlobalContext.FirstPartitionedNodePort);
        private readonly BenchmarkGlobalContext _context = new BenchmarkGlobalContext();
        public int NbMessageSent { get; set; } = 0;
        public int NbMessageReceived { get; set; } = 1;

        public async Task Launch(IEnumerable<IBenchmarkScenario> scenarios)
        {
            await GlobalSetup();
            foreach (var scenario in scenarios)
                await ExecuteScenario(scenario);
            GlobalCleanup();
        }

        private async Task ExecuteScenario(IBenchmarkScenario scenario)
        {
            await scenario.Setup(_context);
            await scenario.Execute(_context);
            scenario.Cleanup();
        }

        private async Task GlobalSetup()
        {
            _serverPool.AddPartitionedNode(BenchmarkGlobalContext.FirstPartitionedNodePort, 30000);
            _serverPool.AddPartitionedNode(BenchmarkGlobalContext.SecondPartitionedNodePort, 40000);
            await _serverPool.Start();
            await _serverPool.WaitAllStandardPartitionsAreLaunched();
            await _client.AddVpn(BenchmarkGlobalContext.DefaultVpn);
            _context.PubClient = await _client.AddPubClient("pubClient", BenchmarkGlobalContext.DefaultVpn);
            _context.SubClient = await _client.AddSubClient("subClient", BenchmarkGlobalContext.DefaultVpn);
            await _client.AddQueue(BenchmarkGlobalContext.DefaultQueueName, BenchmarkGlobalContext.DefaultVpn);
            await _serverPool.WaitPartition($"{BenchmarkGlobalContext.DefaultVpn}_{BenchmarkGlobalContext.DefaultQueueName}");
            await _client.AddSubscription(BenchmarkGlobalContext.DefaultQueueName, BenchmarkGlobalContext.DefaultMessageTopic, BenchmarkGlobalContext.DefaultVpn);
        }

        private void GlobalCleanup() { }
    }
}
