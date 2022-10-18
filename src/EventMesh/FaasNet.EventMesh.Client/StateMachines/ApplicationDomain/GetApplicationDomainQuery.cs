using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class GetApplicationDomainQuery : IQuery
    {
        public string Vpn { get; set; }
        public string Name { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            Name = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(Name);
        }
    }
}
