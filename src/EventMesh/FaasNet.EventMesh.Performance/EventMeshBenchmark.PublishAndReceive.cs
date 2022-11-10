using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using System.Diagnostics;

namespace FaasNet.EventMesh.Performance
{
    public partial class EventMeshBenchmark
    {
        private EventMeshSubscribeSessionClient _subSessionClient;
        private EventMeshPublishSessionClient _pubSessionClient;
        private StreamWriter _recordsFile;
        private Stopwatch sw;

        private async Task LaunchPublishAndReceiveMessage(int nbIterations = 100)
        {
            await LaunchPublishAndReceiveSetup();
            await ExecuteOperation(PublishAndReceiveMessage, nbIterations);
            LaunchPublishAndReceiveCleanup();
        }

        private async Task LaunchPublishAndReceiveSetup()
        {
            sw = new Stopwatch();
            _subClientCancellationTokenSource = new CancellationTokenSource();
            _subSessionClient = await _eventMeshClient.CreateSubSession(_addSubClient.ClientId, vpn, _addSubClient.ClientSecret, _addSubClient.ClientId);
            _pubSessionClient = await _eventMeshClient.CreatePubSession(_addPubClient.ClientId, vpn, _addPubClient.ClientSecret);
#pragma warning disable 4014
            Task.Run(ListenMessages, _subClientCancellationTokenSource.Token);
#pragma warning restore 4014
            if (File.Exists(Constants.RecordsFilePath)) File.Delete(Constants.RecordsFilePath);
            _recordsFile = new StreamWriter(Constants.RecordsFilePath);
            _recordsFile.WriteLine($"NbRequestSent{Constants.Separator}Timestamp{Constants.Separator}NbRequestReceived");
            sw.Start();
        }

        private void LaunchPublishAndReceiveCleanup()
        {
            sw.Stop();
            _subClientCancellationTokenSource.Cancel();
            _recordsFile.Dispose();
        }

        private async Task PublishAndReceiveMessage()
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
                var r = await _pubSessionClient.PublishMessage(messageTopic, cloudEvent);
                return (r, r.Status == Client.Messages.PublishMessageStatus.SUCCESS);
            });
            NbMessageSent++;
            var record = $"{NbMessageSent}{Constants.Separator}{sw.ElapsedMilliseconds}{Constants.Separator}{NbMessageReceived}";
            File.WriteAllText(Constants.SummaryFilePath, record);
            _recordsFile.WriteLine(record);
        }

        private async void ListenMessages()
        {
            try
            {
                while (true)
                {
                    _subClientCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
}
