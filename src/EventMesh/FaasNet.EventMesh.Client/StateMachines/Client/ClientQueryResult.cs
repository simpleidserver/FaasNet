using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class ClientQueryResult : ISerializable
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public string Vpn { get; set; }
        public int SessionExpirationTimeMS { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; } = new List<ClientPurposeTypes>();
        public DateTime? CreateDateTime { get; set; }
        public ICollection<ClientLinkResult> Sources { get; set; } = new List<ClientLinkResult>();
        public ICollection<ClientLinkResult> Targets { get; set; } = new List<ClientLinkResult>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientSecret = context.NextString();
            Vpn = context.NextString();
            SessionExpirationTimeMS = context.NextInt();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) Purposes.Add((ClientPurposeTypes)context.NextInt());
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var source = new ClientLinkResult();
                source.Deserialize(context);
                Sources.Add(source);
            }

            nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var target = new ClientLinkResult();
                target.Deserialize(context);
                Targets.Add(target);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientSecret);
            context.WriteString(Vpn);
            context.WriteInteger(SessionExpirationTimeMS);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.GetValueOrDefault().Ticks));
            context.WriteInteger(Sources.Count());
            foreach (var source in Sources) source.Serialize(context);
            context.WriteInteger(Targets.Count());
            foreach (var target in Targets) target.Serialize(context);
        }
    }

    public class ClientLinkResult : ISerializable
    {
        public string ClientId { get; set; }
        public string EventId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            EventId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(EventId);
        }
    }
}
