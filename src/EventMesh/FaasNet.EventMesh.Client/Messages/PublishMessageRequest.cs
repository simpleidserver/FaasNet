using CloudNative.CloudEvents;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class PublishMessageRequest : BaseEventMeshPackage
    {
        public PublishMessageRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.PUBLISH_MESSAGE_REQUEST;
        public string SessionId { get; set; }
        public string Topic { get; set; }
        public CloudEvent CloudEvent { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
