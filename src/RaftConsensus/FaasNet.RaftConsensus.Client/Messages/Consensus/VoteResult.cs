namespace FaasNet.RaftConsensus.Client.Messages.Consensus
{
    public class VoteResult : ConsensusPackage
    {
        public bool VoteGranted { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteBoolean(VoteGranted);
        }

        public void Extract(ReadBufferContext context)
        {
            VoteGranted = context.NextBoolean();
        }
    }
}
