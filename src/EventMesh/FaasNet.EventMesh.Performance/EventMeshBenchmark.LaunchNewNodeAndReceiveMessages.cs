using FaasNet.EventMesh.Client;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;

namespace FaasNet.EventMesh.Performance
{
    public partial class EventMeshBenchmark
    {
        private StreamWriter _recordsLaunchNewNodeFile;

        private async Task LaunchNewNodeAndReceiveMessage(int nbIterations = 100)
        {
            NbMessageReceived = 1;
            await LaunchNewNodeAndReceiveMessageSetup();
            while(true)
            {
                if (NbMessageReceived == nbIterations) break;
                var r = await _subSessionClient.ReadMessage(NbMessageReceived);
                if (r.Status == Client.Messages.ReadMessageStatus.SUCCESS)
                {
                    var record = $"{sw.ElapsedMilliseconds}{Constants.Separator}{NbMessageReceived}";
                    _recordsLaunchNewNodeFile.WriteLine(record);
                    Console.WriteLine($"Receive {NbMessageReceived} in new node");
                    NbMessageReceived++;
                }
            }
            LaunchNewNodeAndReceiveMessageCleanup();
        }

        private async Task LaunchNewNodeAndReceiveMessageSetup()
        {
            _recordsLaunchNewNodeFile = new StreamWriter(Constants.RecordsNewNodeFilePath);
            _recordsLaunchNewNodeFile.WriteLine($"Timestamp{Constants.Separator}NbRequestReceived");
            var thirdNode = await NodeHelper.BuildAndStartNode(5002, clusterNodes, 50000);
            _eventMeshClient = PeerClientFactory.Build<EventMeshClient>("localhost", 5002, new ClientUDPTransport());
            _subSessionClient = await Retry(async () =>
            {
                try
                {
                    var result = await _eventMeshClient.CreateSubSession(_addSubClient.ClientId, vpn, _addSubClient.ClientSecret, _addSubClient.ClientId);
                    return (result, result.Session.Status == Client.Messages.HelloMessageStatus.SUCCESS);
                }
                catch
                {
                    return (null, false);
                }
            });
            sw.Reset();
            sw.Start();
        }

        private void LaunchNewNodeAndReceiveMessageCleanup()
        {
            _recordsLaunchNewNodeFile.Dispose();
            sw.Stop();
        }
    }
}
