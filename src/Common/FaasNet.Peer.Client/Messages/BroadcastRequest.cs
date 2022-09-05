using System;

namespace FaasNet.Peer.Client.Messages
{
    public class BroadcastRequest : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.BROADCAST_REQUEST;

        public byte[] Content { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteByteArray(Content);
        }

        public BroadcastRequest Extract(ReadBufferContext context)
        {
            Content = context.NextByteArray();
            return this;
        }
    }
}
