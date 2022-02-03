using System;

namespace EventMesh.Runtime.Messages
{
    public class Header
    {
        public Header(Commands command, HeaderStatus status, string seq)
        {
            Command = command;
            Status = status;
            Seq = seq;
        }

        public Commands Command { get; set; }
        public HeaderStatus Status { get; set; }
        public string Seq { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            Command.Serialize(context);
            Status.Serialize(context);
            context.WriteString(Seq);
        }

        public static Header Deserialize(ReadBufferContext context)
        {
            var cmd = Commands.Deserialize(context);
            var status = HeaderStatus.Deserialize(context);
            var seq = context.NextString();
            return new Header(cmd, status, seq);
        }
    }
}
