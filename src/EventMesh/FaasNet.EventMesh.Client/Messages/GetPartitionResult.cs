using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetPartitionResult : BaseEventMeshPackage
    {
        public GetPartitionResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_PARTITION_RESPONSE;
        public GetPartitionStatus Status { get; set; }
        public GetPeerStateResult State { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            if (Status == GetPartitionStatus.OK) State.SerializeEnvelope(context);
        }

        public GetPartitionResult Extract(ReadBufferContext context)
        {
            Status = (GetPartitionStatus)context.NextInt();
            if(Status == GetPartitionStatus.OK) State = BaseConsensusPackage.Deserialize(context) as GetPeerStateResult;
            return this;
        }
    }

    public enum GetPartitionStatus
    {
        OK = 0,
        NOPARTITION = 1
    }
}
