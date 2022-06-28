namespace FaasNet.DHT.Chord.Client.Messages
{
    public class DHTPackage
    {
        private const string MAGIC_CODE = "DHT";
        private const string PROTOCOL_VERSION = "0000";

        public DHTPackage(Commands command)
        {
            Command = command;
        }

        public Commands Command { get; private set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Command.Serialize(context);
        }

        public static DHTPackage Deserialize(ReadBufferContext context)
        {
            context.NextString();
            context.NextString();
            var command = Commands.Deserialize(context);
            if(command == Commands.GET_DIMENSION_FINGER_TABLE_RESULT)
            {
                var result = new GetDimensionFingerTableResult();
                result.Extract(context);
                return result;
            }

            if (command == Commands.JOIN_CHORD_NETWORK_REQUEST)
            {
                var result = new JoinChordNetworkRequest();
                result.Extract(context);
                return result;
            }

            if (command == Commands.FIND_SUCCESSOR_REQUEST)
            {
                var result = new FindSuccessorRequest();
                result.Extract(context);
                return result;
            }

            if(command == Commands.FIND_SUCCESSOR_RESULT)
            {
                var result = new FindSuccessorResult();
                result.Extract(context);
                return result;
            }

            if(command == Commands.CREATE_REQUEST)
            {
                var result = new CreateRequest();
                result.Extract(context);
                return result;
            }

            if (command == Commands.NOTIFY_REQUEST)
            {
                var result = new NotifyRequest();
                result.Extract(context);
                return result;
            }

            if (command == Commands.FIND_PREDECESSOR_RESULT)
            {
                var result = new FindPredecessorResult();
                result.Extract(context);
                return result;
            }

            return new DHTPackage(command);
        }
    }
}
