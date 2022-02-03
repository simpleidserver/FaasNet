namespace EventMesh.Runtime.Messages
{
    public class Package
    {
        private const string MAGIC_CODE = "EventMesh";
        private const string PROTOCOL_VERSION = "0000";

        public Header Header { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static Package Deserialize(ReadBufferContext context)
        {
            context.NextString();
            context.NextString();
            var header = Header.Deserialize(context);
            if (Commands.HELLO_REQUEST == header.Command)
            {
                var result = new HelloRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.SUBSCRIBE_REQUEST == header.Command)
            {
                var result = new SubscriptionRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            return new Package
            {
                Header = header
            };
        }
    }
}
