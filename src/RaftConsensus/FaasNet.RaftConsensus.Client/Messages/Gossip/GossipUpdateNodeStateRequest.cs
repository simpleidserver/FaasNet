namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipUpdateNodeStateRequest : GossipPackage
    {
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Value { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(EntityType);
            context.WriteString(EntityId);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            EntityType = context.NextString();
            EntityId = context.NextString();
            Value = context.NextString();
        }
    }
}
