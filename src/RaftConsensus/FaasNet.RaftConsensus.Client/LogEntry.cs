using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client
{
    public record LogEntry
    {
        public long Index { get; set; }
        public long Term { get; set; }
        public string StateMachineId { get; set; }
        public byte[] Command { get; set; }

        public static LogEntry Deserialize(byte[] payload)
        {
            var readBufferCtx = new ReadBufferContext(payload);
            return Deserialize(readBufferCtx);
        }

        public byte[] Serialize()
        {
            var writeBufferCtx = new WriteBufferContext();
            Serialize(writeBufferCtx);
            return writeBufferCtx.Buffer.ToArray();
        }

        static internal LogEntry Deserialize(ReadBufferContext bufferCtx)
        {
            return new LogEntry
            {
                Index = bufferCtx.NextLong(),
                Term = bufferCtx.NextLong(),
                StateMachineId = bufferCtx.NextString(),
                Command = bufferCtx.NextByteArray()
            };
        }

        internal void Serialize(WriteBufferContext bufferCtx)
        {
            bufferCtx.WriteLong(Index);
            bufferCtx.WriteLong(Term);
            bufferCtx.WriteString(StateMachineId);
            bufferCtx.WriteByteArray(Command);
        }
    }
}
