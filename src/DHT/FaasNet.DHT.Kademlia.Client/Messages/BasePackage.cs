namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class BasePackage
    {
        private const string MAGIC_CODE = "KADEMLIA";
        private const string PROTOCOL_VERSION = "0000";

        public BasePackage(Commands command)
        {
            Command = command;
        }

        public Commands Command { get; private set; }
        public string Nonce { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Command.Serialize(context);
            context.WriteString(Nonce);
        }

        public virtual BasePackage Extract(ReadBufferContext context)
        {
            Nonce = context.NextString();
            return this;
        }

        public static BasePackage Deserialize(ReadBufferContext context)
        {
            context.NextString();
            context.NextString();
            var cmd = Commands.Deserialize(context);
            BasePackage result = null;
            if(cmd == Commands.FIND_NODE_REQUEST) result = new FindNodeRequest().Extract(context);
            if(cmd == Commands.FIND_NODE_RESULT) result = new FindNodeResult().Extract(context);
            if(cmd == Commands.PING_REQUEST) result = new PingRequest().Extract(context);
            if(cmd == Commands.PING_RESULT) result = new PingResult();
            if(cmd == Commands.FIND_VALUE_REQUEST) result = new FindValueRequest().Extract(context);
            if(cmd == Commands.FIND_VALUE_RESULT) result = new FindValueResult().Extract(context);
            if(cmd == Commands.STORE_REQUEST) result = new StoreRequest().Extract(context);
            if(cmd == Commands.STORE_RESULT) result = new StoreResult();
            if(cmd == Commands.JOIN_REQUEST) result = new JoinRequest().Extract(context);
            if (cmd == Commands.JOIN_RESULT) result = new JoinResult();
            return result;
        }
    }
}
