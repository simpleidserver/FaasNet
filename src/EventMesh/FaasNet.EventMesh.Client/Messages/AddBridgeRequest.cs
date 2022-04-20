
namespace FaasNet.EventMesh.Client.Messages
{
    public class AddBridgeRequest : Package
    {
        #region Properties

        public string Vpn { get; set; }
        public string TargetVpn { get; set; }
        public string TargetUrn { get; set; }
        public int TargetPort { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Vpn);
            context.WriteString(TargetVpn);
            context.WriteString(TargetUrn);
            context.WriteInteger(TargetPort);
        }

        public void Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            TargetVpn = context.NextString();
            TargetUrn = context.NextString();
            TargetPort = context.NextInt();
        }

        public override string ToString()
        {
            return $"Command = {Header.Command}, Vpn = {Vpn}, TargetVpn = {TargetVpn}, TargetUrn = {TargetUrn}, TargetPort = {TargetPort}";
        }
    }
}
