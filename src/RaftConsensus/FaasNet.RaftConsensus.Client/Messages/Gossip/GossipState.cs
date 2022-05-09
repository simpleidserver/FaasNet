namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipState
    {
        public string EntityType { get; set; }
        public int EntityVersion { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(EntityType);
            context.WriteInteger(EntityVersion);
        }

        public static GossipState Deserialize(ReadBufferContext context)
        {
            return new GossipState { EntityType = context.NextString(), EntityVersion = context.NextInt() };
        }
    }
}
