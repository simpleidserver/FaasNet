using FaasNet.EventMesh.Client;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;

namespace FaasNet.EventMesh.Performance
{
    public partial class EventMeshBenchmark
    {
        private async Task LaunchNewNodeAndReceiveMessage(int nbIterations = 100)
        {
            NbMessageReceived = 1;
            await LaunchNewNodeAndReceiveMessageSetup();
            while(true)
            {
                if (NbMessageReceived == nbIterations) break;
                var r = await _subSessionClient.ReadMessage(NbMessageReceived);
                if (r.Status == Client.Messages.ReadMessageStatus.SUCCESS)
                    NbMessageReceived++;
            }
        }

        private async Task LaunchNewNodeAndReceiveMessageSetup()
        {
            var thirdNode = await NodeHelper.BuildAndStartNode(5002, clusterNodes, 50000, (p) =>
            {
                CheckPartitions(p);
            });
            _semStandardPartitionsLaunched.Wait();
            Console.WriteLine("Standard partitions are launched");
            _eventMeshClient = PeerClientFactory.Build<EventMeshClient>("localhost", 5002, new ClientUDPTransport());
            _subSessionClient = await _eventMeshClient.CreateSubSession(_addSubClient.ClientId, vpn, _addSubClient.ClientSecret, _addSubClient.ClientId);
        }
    }
}
