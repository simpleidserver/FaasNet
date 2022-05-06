namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GossipHeartbeatRequest : GossipPackage
    {
        public string EntityType { get; set; }
        public int EntityVersion { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(EntityType);
            context.WriteInteger(EntityVersion);
        }

        public void Extract(ReadBufferContext context)
        {
            EntityType = context.NextString();
            EntityVersion = context.NextInt();
        }
    }
}
