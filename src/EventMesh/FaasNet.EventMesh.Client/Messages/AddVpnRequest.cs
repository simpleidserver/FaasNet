namespace FaasNet.EventMesh.Client.Messages
{
    public class AddVpnRequest : Package
    {
        public string Vpn { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Vpn);
        }

        public void Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
        }
    }
}
