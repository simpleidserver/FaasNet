using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public abstract class ChordPackage : BasePeerPackage
    {
        public const string MAGIC_CODE = "CHORD";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => "0000";
        public abstract ChordCommandTypes Command { get; }

        public override void SerializeBody(WriteBufferContext context)
        {
            Command.Serialize(context);
            SerializeAction(context);
        }

        public abstract void SerializeAction(WriteBufferContext context);

        public static ChordPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if (!ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var command = ChordCommandTypes.Deserialize(context);
            if (command == ChordCommandTypes.EMPTY_RESULT) return new EmptyChordPackage();
            if (command == ChordCommandTypes.ADD_KEY_RESULT) return new AddKeyResult();
            if (command == ChordCommandTypes.NOTIFY_RESULT) return new NotifyResult();
            if (command == ChordCommandTypes.FIND_PREDECESSOR_REQUEST) return new FindPredecessorRequest();
            if (command == ChordCommandTypes.GET_DIMENSION_FINGER_TABLE_REQUEST) return new GetDimensionFingerTableRequest();
            if (command == ChordCommandTypes.GET_DIMENSION_FINGER_TABLE_RESULT)
            {
                var result = new GetDimensionFingerTableResult();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.JOIN_CHORD_NETWORK_REQUEST)
            {
                var result = new JoinChordNetworkRequest();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.FIND_SUCCESSOR_REQUEST)
            {
                var result = new FindSuccessorRequest();
                result.Extract(context);
                return result;
            }

            if(command == ChordCommandTypes.FIND_SUCCESSOR_RESULT)
            {
                var result = new FindSuccessorResult();
                result.Extract(context);
                return result;
            }

            if(command == ChordCommandTypes.CREATE_REQUEST)
            {
                var result = new CreateRequest();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.NOTIFY_REQUEST)
            {
                var result = new NotifyRequest();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.FIND_PREDECESSOR_RESULT)
            {
                var result = new FindPredecessorResult();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.ADD_KEY_REQUEST)
            {
                var result = new AddKeyRequest();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.GET_KEY_REQUEST)
            {
                var result = new GetKeyRequest();
                result.Extract(context);
                return result;
            }

            if (command == ChordCommandTypes.GET_KEY_RESULT)
            {
                var result = new GetKeyResult();
                result.Extract(context);
                return result;
            }

            return null;
        }
    }
}
