using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class FindVpnsByNameRequest : BaseEventMeshPackage
    {
        public FindVpnsByNameRequest(string seq) : base(seq)
        {
        }

        public string Name { get; set; }
        public override EventMeshCommands Command => EventMeshCommands.FIND_VPNS_BY_NAME_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
        }

        public FindVpnsByNameRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            return this;
        }
    }
}
