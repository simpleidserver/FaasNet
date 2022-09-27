using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class FindClientsByNameRequest : BaseEventMeshPackage
    {
        public FindClientsByNameRequest(string seq) : base(seq)
        {
        }

        public string Name { get; set; }
        public override EventMeshCommands Command => EventMeshCommands.FIND_CLIENTS_BY_NAME_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
        }

        public FindClientsByNameRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            return this;
        }
    }
}
