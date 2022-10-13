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
        public ICollection<string> Sources { get; set; } = new List<string>();
        public ICollection<string> Targets { get; set; } = new List<string>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            JsonSchema = context.NextString();
            var nbSources = context.NextInt();
            for (var i = 0; i < nbSources; i++) Sources.Add(context.NextString());
            var nbTargets = context.NextInt();
            for (var i = 0; i < nbTargets; i++) Targets.Add(context.NextString());
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteString(JsonSchema);
            context.WriteInteger(Sources.Count);
            foreach (var source in Sources) context.WriteString(source);
            context.WriteInteger(Targets.Count);
            foreach (var target in Targets) context.WriteString(target);
        }
    }
}
