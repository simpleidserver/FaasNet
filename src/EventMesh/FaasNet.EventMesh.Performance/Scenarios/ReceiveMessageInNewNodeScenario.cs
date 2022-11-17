using FaasNet.EventMesh.Client;
using System.Diagnostics;

namespace FaasNet.EventMesh.Performance.Scenarios
{
    public class ReceiveMessageInNewNodeScenario : IBenchmarkScenario
    {
        private int _nbMessageReceived = 1;
        private static int DEFAULT_PORT = 5002;
        private readonly Stopwatch _sw = new Stopwatch();
        private readonly EventMeshPartitionedPeerPool _serverPool = EventMeshPartitionedPeerPool.Create();
        private readonly EventMeshClientHelper _client = EventMeshClientHelper.CreateUDPClient(BenchmarkGlobalContext.Url, DEFAULT_PORT);
        private StreamWriter _recordsFile = null!;
        private EventMeshSubscribeSessionClient _subSession = null!;

        public async Task Setup(BenchmarkGlobalContext context)
        {
            _serverPool.AddPartitionedNode(DEFAULT_PORT, 50000);
            await _serverPool.Start();
            _subSession = await _client.CreateSubSession(context.SubClient.ClientId, context.SubClient.ClientSecret, BenchmarkGlobalContext.DefaultQueueName, BenchmarkGlobalContext.DefaultVpn);
            InitFile();
            _sw.Start();

            void InitFile()
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "PublishAndReceiveMessageScenario.txt");
                if (File.Exists(path)) File.Delete(path);
                _recordsFile = new StreamWriter(path);
                _recordsFile.WriteLine("Timestamp;NbRequestReceived");
            }
        }

        public async Task Execute(BenchmarkGlobalContext context)
        {
            while(true)
            {
                var r = await _subSession.ReadMessage(_nbMessageReceived);
                if(r.Status == Client.Messages.ReadMessageStatus.SUCCESS)
                {
                    _recordsFile.WriteLine($"{_sw.ElapsedMilliseconds};{_nbMessageReceived}");
                    Console.WriteLine($"Receive {_nbMessageReceived} in new node");
                    _nbMessageReceived++;
                }
            }
        }

        public void Cleanup()
        {
            _sw.Stop();
            _recordsFile.Dispose();
        }
    }
}
