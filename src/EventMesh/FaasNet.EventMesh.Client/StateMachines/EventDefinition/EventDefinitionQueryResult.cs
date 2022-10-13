using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.EventDefinition
{
    public class EventDefinitionQueryResult : ISerializable
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string JsonSchema { get; set; }
        public ICollection<EventDefinitionLinkResult> Links { get; set; } = new List<EventDefinitionLinkResult>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            JsonSchema = context.NextString();
            var nbLinks = context.NextInt();
            for (var i = 0; i < nbLinks; i++)
            {
                var link = new EventDefinitionLinkResult();
                link.Deserialize(context);
                Links.Add(link);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteString(JsonSchema);
            context.WriteInteger(Links.Count);
            foreach (var link in Links) link.Serialize(context);
        }
    }

    public class EventDefinitionLinkResult : ISerializable
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Source = context.NextString();
            Target = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Source);
            context.WriteString(Target);
        }
    }
}
