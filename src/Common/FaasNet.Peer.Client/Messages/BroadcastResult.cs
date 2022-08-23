using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Peer.Client.Messages
{
    public class BroadcastResult : BasePartitionedRequest
    {
        public BroadcastResult()
        {
            ContentLst = new List<byte[]>();
        }

        public override PartitionedCommands Command => PartitionedCommands.BROADCAST_RESULT;

        public IEnumerable<byte[]> ContentLst { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(ContentLst.Count());
            foreach(var content in ContentLst) context.WriteByteArray(content);
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<byte[]>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) result.Add(context.NextByteArray());
            ContentLst = result;
        }
    }
}
