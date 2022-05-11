namespace FaasNet.EventMesh.Client.Messages
{
    public class AddClientRequest : Package
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Vpn);
            context.WriteString(ClientId);
        }

        public void Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            ClientId = context.NextString();
        }
    }
}
