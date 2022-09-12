
namespace FaasNet.Peer.Client.Messages
{
    public class PartitionPackageRequestBuilder
    {
        public static BasePartitionedRequest AddPartition(string partitionKey, string partitionType)
        {
            return new AddDirectPartitionRequest
            {
                PartitionKey = partitionKey,
                StateMachineType = partitionType
            };
        }

        public static BasePartitionedRequest RemovePartition(string partitionKey)
        {
            return new RemoveDirectPartitionRequest
            {
                PartitionKey = partitionKey
            };
        }

        public static BasePartitionedRequest Transfer(string partitionKey, byte[] content)
        {
            return new TransferedRequest
            {
                Content = content,
                PartitionKey = partitionKey
            };
        }

        public static BasePartitionedRequest Broadcast(byte[] content)
        {
            return new BroadcastRequest
            {
                Content = content
            };
        }

        public static BasePartitionedRequest GetAllNodes()
        {
            return new GetAllNodesRequest();
        }
    }
}
