namespace FaasNet.EventMesh.Runtime.Messages
{
    public class Header
    {
        public Header(Commands command, HeaderStatus status, string seq, Errors error = null)
        {
            Command = command;
            Status = status;
            Seq = seq;
            Error = error;
        }

        public Commands Command { get; set; }
        public HeaderStatus Status { get; set; }
        public Errors Error { get; set; }
        public string Seq { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            Command.Serialize(context);
            Status.Serialize(context);
            if (Status != HeaderStatus.SUCCESS)
            {
                Error.Serialize(context);
            }

            context.WriteString(Seq);
        }

        public static Header Deserialize(ReadBufferContext context)
        {
            var cmd = Commands.Deserialize(context);
            var status = HeaderStatus.Deserialize(context);
            Errors error = null;
            if (status != HeaderStatus.SUCCESS)
            {
                error = Errors.Deserialize(context);
            }

            var seq = context.NextString();
            return new Header(cmd, status, seq, error);
        }
    }
}
