using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Peer.Client.Messages
{
    public class BroadcastResult : BasePartitionedRequest
    {
        public BroadcastResult()
        {
            ContentLst = new List<BroadcastRecordResult>();
        }

        public override PartitionedCommands Command => PartitionedCommands.BROADCAST_RESULT;

        public ICollection<BroadcastRecordResult> ContentLst { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(ContentLst.Count());
            foreach (var content in ContentLst) content.Serialize(context);
        }

        public BroadcastResult Extract(ReadBufferContext context)
        {
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) ContentLst.Add(BroadcastRecordResult.Deserialize(context));
            return this;
        }
    }

    public class BroadcastRecordResult
    {
        public string PartitionKey { get; set; }
        public byte[] Content { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(PartitionKey);
            context.WriteByteArray(Content);
        }

        public static BroadcastRecordResult Deserialize(ReadBufferContext context)
        {
            return new BroadcastRecordResult
            {
                PartitionKey = context.NextString(),
                Content = context.NextByteArray()
            };
        }
    }
}
