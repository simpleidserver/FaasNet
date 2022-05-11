namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipAddPeerRequest : GossipPackage
    {
        public string TermId { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(TermId);
        }

        public void Extract(ReadBufferContext context)
        {
            TermId = context.NextString();
        }
    }
}
