using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.Subscription
{
    public class RemoveSubscriptionCommand : ICommand
    {
        public string ClientId { get; set; }
        public string EventId { get; set; }
        public string Vpn { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            EventId = context.NextString();
            Vpn = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(EventId);
            context.WriteString(Vpn);
        }
    }
}