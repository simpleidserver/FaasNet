
using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class DisconnectRequest : Package
    {
        #region Properties

        public string ClientId { get; set; }
        public string SessionId { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteString(SessionId);
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            SessionId = context.NextString();
        }
    }
}
