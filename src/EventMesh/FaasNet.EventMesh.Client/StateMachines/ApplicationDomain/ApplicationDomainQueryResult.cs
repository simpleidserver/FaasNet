using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class ApplicationDomainQueryResult : ISerializable
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            Description = context.NextString();
            RootTopic = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            UpdateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(Description);
            context.WriteString(RootTopic);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Value.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Value.Ticks));
        }
    }
}
