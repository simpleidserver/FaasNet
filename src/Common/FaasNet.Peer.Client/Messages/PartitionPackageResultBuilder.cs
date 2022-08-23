
using System.Collections.Generic;

namespace FaasNet.Peer.Client.Messages
{
    public static class PartitionPackageResultBuilder
    {
        public static BasePartitionedRequest AddPartition()
        {
            return new AddDirectPartitionResult();
        }

        public static BasePartitionedRequest Broadcast(IEnumerable<byte[]> contentLst)
        {
            return new BroadcastResult
            {
                ContentLst = contentLst
            };
        }
    }
}
