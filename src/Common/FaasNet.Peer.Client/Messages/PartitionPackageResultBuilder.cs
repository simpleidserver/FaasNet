
using System.Collections.Generic;

namespace FaasNet.Peer.Client.Messages
{
    public static class PartitionPackageResultBuilder
    {
        public static BasePartitionedRequest AddPartition(AddDirectPartitionStatus status)
        {
            return new AddDirectPartitionResult { Status = status };
        }

        public static BasePartitionedRequest RemovePartition(RemoveDirectPartitionStatus status)
        {
            return new RemoveDirectPartitionResult
            {
                Status = status
            };
        }

        public static BasePartitionedRequest Broadcast(ICollection<BroadcastRecordResult> contentLst)
        {
            return new BroadcastResult
            {
                ContentLst = contentLst
            };
        }

        public static BasePartitionedRequest GetAllNodes(ICollection<NodeResult> nodes)
        {
            return new GetAllNodesResult
            {
                Nodes = nodes
            };
        }
    }
}
