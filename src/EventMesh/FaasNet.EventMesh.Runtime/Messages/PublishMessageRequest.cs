using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Extensions;

namespace FaasNet.EventMesh.Runtime.Messages
{
    public class PublishMessageRequest : Package
    {
        #region Properties

        public string ClientId { get; set; }
        public string SessionId { get; set; }
        public string Topic { get; set; }
        public string Urn { get; set; }
        public int Port { get; set; }
        public CloudEvent CloudEvent { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteString(SessionId);
            context.WriteString(Topic);
            context.WriteString(Urn);
            context.WriteInteger(Port);
            CloudEvent.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            SessionId = context.NextString();
            Topic = context.NextString();
            Urn = context.NextString();
            Port = context.NextInt();
            CloudEvent = context.DeserializeCloudEvent();
        }
    }
}
