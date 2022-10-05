using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class RemoveClientCommand : ICommand
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId).WriteString(Vpn);
        }
    }
}
