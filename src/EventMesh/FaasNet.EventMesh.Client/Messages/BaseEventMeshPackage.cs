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
            return null;
        }
    }
}
