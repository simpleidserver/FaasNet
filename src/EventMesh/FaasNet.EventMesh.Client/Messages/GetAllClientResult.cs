using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllClientResult : BaseEventMeshPackage
    {
        public GetAllClientResult(string seq) : base(seq)
        {
            Clients = new List<ClientResult>();
        }

        public ICollection<ClientResult> Clients { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_CLIENT_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Clients.Count);
            foreach (var client in Clients) client.Serialize(context);
        }

        public GetAllClientResult Extract(ReadBufferContext context)
        {
            int nb = context.NextInt();
            for (var i = 0; i < nb; i++) Clients.Add(ClientResult.Extract(context));
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

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
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
            return result;
        }
    }
}
