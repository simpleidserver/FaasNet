using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public abstract class KademliaPackage : BasePeerPackage
    {
        public const string MAGIC_CODE = "KADEMLIA";
        private const string PROTOCOL_VERSION = "0000";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => PROTOCOL_VERSION;
        public abstract KademliaCommandTypes Command { get; }
        public string Nonce { get; set; }

        public override void SerializeBody(WriteBufferContext context)
        {
            Command.Serialize(context);
            context.WriteString(Nonce);
            SerializeAction(context);
        }

        public abstract void SerializeAction(WriteBufferContext context);

        public static KademliaPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if(!ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var cmd = KademliaCommandTypes.Deserialize(context);
            var nonce = context.NextString();
            if (cmd == KademliaCommandTypes.FIND_NODE_REQUEST)
            {
                var result = new FindNodeRequest { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (cmd == KademliaCommandTypes.FIND_NODE_RESULT)
            {
                var result = new FindNodeResult { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (cmd == KademliaCommandTypes.PING_REQUEST)
            {
                var result = new PingRequest { Nonce = nonce };
                return result;
            }

            if(cmd == KademliaCommandTypes.PING_RESULT)
            {
                var result = new PingResult { Nonce = nonce };
                return result;
            }

            if(cmd == KademliaCommandTypes.FIND_VALUE_REQUEST)
            {
                var result = new FindValueRequest { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (cmd == KademliaCommandTypes.FIND_VALUE_RESULT)
            {
                var result = new FindValueResult { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (cmd == KademliaCommandTypes.STORE_REQUEST)
            {
                var result = new StoreRequest { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (cmd == KademliaCommandTypes.STORE_RESULT)
            {
                var result = new StoreResult { Nonce = nonce };
                return result;
            }

            return null;
        }
    }
}
