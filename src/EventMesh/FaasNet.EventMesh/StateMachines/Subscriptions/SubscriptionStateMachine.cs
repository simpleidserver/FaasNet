using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Core.StateMachines;

namespace FaasNet.EventMesh.StateMachines.Subscriptions
{
    public class SubscriptionStateMachine
    {
    }

    public class SubscriptionRecord : IRecord
    {
        public string ClientId { get; set; }
        public string EventId { get; set; }
        public string Vpn { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(WriteBufferContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
