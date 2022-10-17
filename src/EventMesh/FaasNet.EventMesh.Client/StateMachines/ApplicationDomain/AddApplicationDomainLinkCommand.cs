using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class AddApplicationDomainLinkCommand : ICommand
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            Source = context.NextString();
            Target = context.NextString();
            EventId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(Source);
            context.WriteString(Target);
            context.WriteString(EventId);
        }
    }
}
