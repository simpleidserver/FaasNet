using FaasNet.EventMesh.Client.Messages;

namespace FaasNet.EventMesh.Performance
{
    public sealed class BenchmarkGlobalContext
    {
        public const string Url = "localhost";
        public const string DefaultVpn = "vpn";
        public const string DefaultMessageTopic = "messageTopic";
        public const string DefaultQueueName = "queueName";
        public const int FirstPartitionedNodePort = 5000;
        public const int SecondPartitionedNodePort = 5001;
        public AddClientResult PubClient { get; set; } = null!;
        public AddClientResult SubClient { get; set; } = null!;
    }
}
