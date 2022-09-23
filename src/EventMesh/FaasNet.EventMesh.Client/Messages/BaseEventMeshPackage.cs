using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public abstract class BaseEventMeshPackage : BasePeerPackage
    {
        public static string MAGIC_CODE = "EventMesh";
        public static string VERSION = "0000";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => VERSION;
        public abstract EventMeshCommands Command { get; }
        public string Seq { get; private set; }

        public BaseEventMeshPackage(string seq)
        {
            Seq = seq;
        }

        public override void SerializeBody(WriteBufferContext context)
        {
            Command.Serialize(context);
            context.WriteString(Seq);
            SerializeAction(context);
        }

        protected abstract void SerializeAction(WriteBufferContext context);

        public static BaseEventMeshPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if (!ignoreEnvelope)
            {
                var magicCode = context.NextString();
                var version = context.NextString();
                if (magicCode != MAGIC_CODE || version != VERSION) return null;
            }

            var cmd = EventMeshCommands.Deserialize(context);
            var seq = context.NextString();
            if (cmd == EventMeshCommands.HEARTBEAT_REQUEST) return new PingRequest(seq);
            if (cmd == EventMeshCommands.HEARTBEAT_RESPONSE) return new PingResult(seq);
            if (cmd == EventMeshCommands.ADD_VPN_REQUEST) return new AddVpnRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_VPN_RESPONSE) return new AddVpnResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_VPN_REQUEST) return new GetAllVpnRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_VPN_RESPONSE) return new GetAllVpnResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_CLIENT_REQUEST) return new AddClientRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_CLIENT_RESPONSE) return new AddClientResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_CLIENT_REQUEST) return new GetAllClientRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_CLIENT_RESPONSE) return new GetAllClientResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_QUEUE_REQUEST) return new AddQueueRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_QUEUE_RESPONSE) return new AddQueueResponse(seq).Extract(context);
            if (cmd == EventMeshCommands.PUBLISH_MESSAGE_REQUEST) return new PublishMessageRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.PUBLISH_MESSAGE_RESPONSE) return new PublishMessageResult(seq).Extract(context);
            if (cmd == EventMeshCommands.HELLO_REQUEST) return new HelloRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.HELLO_RESPONSE) return new HelloResult(seq).Extract(context);
            if (cmd == EventMeshCommands.READ_MESSAGE_REQUEST) return new ReadMessageRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.READ_MESSAGE_RESPONSE) return new ReadMessageResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_CLIENT_REQUEST) return new GetClientRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_CLIENT_RESPONSE) return new GetClientResult(seq).Extract(context);
            return null;
        }
    }
}
