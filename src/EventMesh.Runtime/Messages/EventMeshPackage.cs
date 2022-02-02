namespace EventMesh.Runtime.Messages
{
    public class EventMeshPackage
    {
        private const string MAGIC_CODE = "EventMesh";
        private const string PROTOCOL_VERSION = "0000";

        public EventMeshHeader Header { get; set; }

        public void Serialize(EventMeshWriterBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static EventMeshPackage Deserialize(EventMeshReaderBufferContext context)
        {
            context.NextString();
            context.NextString();
            var result = new EventMeshPackage();
            result.Header = EventMeshHeader.Deserialize(context);
            return result;
        }
    }
}
