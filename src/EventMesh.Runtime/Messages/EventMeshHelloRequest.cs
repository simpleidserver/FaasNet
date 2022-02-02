namespace EventMesh.Runtime.Messages
{
    public class EventMeshHelloRequest : EventMeshPackage
    {
        public EventMeshUserAgent UserAgent { get; set; }

        public void Serialize(EventMeshWriterBufferContext context)
        {

        }
    }
}
