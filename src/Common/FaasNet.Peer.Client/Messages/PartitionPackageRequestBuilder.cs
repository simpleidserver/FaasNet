
namespace FaasNet.Peer.Client.Messages
{
    public class PartitionPackageRequestBuilder
    {
        public static BasePartitionedRequest AddPartition(string partitionKey)
        {
            return new AddDirectPartitionRequest
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
    }
}
