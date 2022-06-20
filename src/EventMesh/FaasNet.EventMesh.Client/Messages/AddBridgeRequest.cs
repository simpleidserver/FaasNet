using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddBridgeRequest : Package
    {
        #region Properties

        public string SourceVpn { get; set; }
        public string TargetVpn { get; set; }
        public string TargetUrn { get; set; }
        public int TargetPort { get; set; }
        public string TargetClientId { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(SourceVpn);
            context.WriteString(TargetVpn);
            context.WriteString(TargetUrn);
            context.WriteInteger(TargetPort);
            context.WriteString(TargetClientId);
        }

        public void Extract(ReadBufferContext context)
        {
            SourceVpn = context.NextString();
            TargetVpn = context.NextString();
            TargetUrn = context.NextString();
            TargetPort = context.NextInt();
            TargetClientId = context.NextString();
        }
    }
}
