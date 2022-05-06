namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GossipHeartbeatResult : GossipPackage
    {
        public string EntityType { get; set; }
        public int EntityVersion { get; set; }
        public string Value { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(EntityType);
            context.WriteInteger(EntityVersion);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            EntityType = context.NextString();
            EntityVersion = context.NextInt();
            Value = context.NextString();
        }
    }
}
