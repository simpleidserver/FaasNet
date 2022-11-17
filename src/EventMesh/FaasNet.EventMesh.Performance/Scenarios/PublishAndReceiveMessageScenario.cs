using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using System.Diagnostics;

namespace FaasNet.EventMesh.Performance.Scenarios
{
    public class PublishAndReceiveMessageScenario : IBenchmarkScenario
    {
        private int _nbIterations = 100;
        private StreamWriter _recordsFile=  null!;
        private int _nbMessageReceived = 1;
        private int _nbMessageSent = 0;
        private Stopwatch _sw = new Stopwatch();
        private readonly EventMeshClientHelper _client = EventMeshClientHelper.CreateUDPClient(BenchmarkGlobalContext.Url, BenchmarkGlobalContext.FirstPartitionedNodePort);
        private EventMeshPublishSessionClient _pubSession = null!;
        private EventMeshSubscribeSessionClient _subSession = null!;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public PublishAndReceiveMessageScenario(int nbIterations = 100)
        {
            _nbIterations = nbIterations;
        }

        public async Task Setup(BenchmarkGlobalContext context)
        {
            _pubSession = await _client.CreatePubSession(context.PubClient.ClientId, context.PubClient.ClientSecret, BenchmarkGlobalContext.DefaultVpn);
            _subSession = await _client.CreateSubSession(context.SubClient.ClientId, context.SubClient.ClientSecret, BenchmarkGlobalContext.DefaultQueueName, BenchmarkGlobalContext.DefaultVpn);
#pragma warning disable 4014
            Task.Run(ListenMessages, _tokenSource.Token);
#pragma warning restore 4014
            InitFile();
            _sw.Start();

            void InitFile()
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "PublishAndReceiveMessageScenario.txt");
                if (File.Exists(path)) File.Delete(path);
                _recordsFile = new StreamWriter(path);
                _recordsFile.WriteLine("NbRequestSent;Timestamp;NbRequestReceived");
            }
        }

        public async Task Execute(BenchmarkGlobalContext context)
        {
            for(var i = 0; i < _nbIterations; i++)
            {
                var cloudEvent = new CloudEvent
                {
                    Type = "com.github.pull.create",
                    Source = new Uri("https://github.com/cloudevents/spec/pull"),
                    Subject = "123",
                    Id = "A234-1234-1234",
                    Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                    DataContentType = "application/json",
                    Data = $"Message number {_nbMessageSent}",
                    ["comexampleextension1"] = "value"
                };
                await _pubSession.PublishMessage(BenchmarkGlobalContext.DefaultMessageTopic, cloudEvent);
                _nbMessageSent++;
                WriteRecord();
            }

            void WriteRecord()
            {
                var record = $"{_nbMessageSent};{_sw.ElapsedMilliseconds};{_nbMessageReceived}";
                _recordsFile.WriteLine(record);
            }
        }

        public void Cleanup()
        {
            _sw.Stop();
            _tokenSource.Cancel();
            _recordsFile.Dispose();
        }

        private async void ListenMessages()
        {
            try
            {
                while (true)
                {
                    _tokenSource.Token.ThrowIfCancellationRequested();
                    var r = await _subSession.ReadMessage(_nbMessageReceived);
                    if (r.Status == Client.Messages.ReadMessageStatus.SUCCESS)
                        _nbMessageReceived++;
                }
            }
            catch
            {

            }
        }
    }
}
