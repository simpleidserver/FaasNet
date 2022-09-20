using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using System;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllClientResult : BaseEventMeshPackage
    {
        public GetAllClientResult(string seq) : base(seq) { }

        public GenericSearchQueryResult<ClientQueryResult> Content { get; set; } = new GenericSearchQueryResult<ClientQueryResult>();

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_CLIENT_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            Content.Serialize(context);
        }

        public GetAllClientResult Extract(ReadBufferContext context)
        {
            Content.Deserialize(context);
            return this;
        }
    }

    public class ClientResult
    {
        public ClientResult()
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }
        public DateTime CreateDateTime { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
        }

        public static ClientResult Extract(ReadBufferContext context)
        {
            var result = new ClientResult
            {
                Id = context.NextString(),
                Vpn = context.NextString()
            };
            var nbPurposes = context.NextInt();
            for (var i = 0; i < nbPurposes; i++) result.Purposes.Add((ClientPurposeTypes)context.NextInt());
            result.CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            return result;
        }
    }
}
