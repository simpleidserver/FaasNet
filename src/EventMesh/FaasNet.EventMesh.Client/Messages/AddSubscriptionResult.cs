using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddSubscriptionResult : BaseEventMeshPackage
    {
        public AddSubscriptionResult(string seq) : base(seq)
        {
        }

        public AddSubscriptionStatus Status { get; set; }
        public string SubscriptionId { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.ADD_SUBSCRIPTION_RESULT;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            if (Status == AddSubscriptionStatus.OK) context.WriteString(SubscriptionId);
        }

        public AddSubscriptionResult Extract(ReadBufferContext context)
        {
            Status = (AddSubscriptionStatus)context.NextInt();
            if (Status == AddSubscriptionStatus.OK) SubscriptionId = context.NextString();
            return this;
        }
    }

    public enum AddSubscriptionStatus
    {
        OK = 0,
        UNKNOWN_VPN = 1,
        UNKNOWN_QUEUE = 2
    }
}
