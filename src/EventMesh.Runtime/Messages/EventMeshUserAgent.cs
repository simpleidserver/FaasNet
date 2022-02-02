namespace EventMesh.Runtime.Messages
{
    public class EventMeshUserAgent
    {
        public string Environment { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public int Port { get; set; }
        public int Pid { get; set; }

        public void Serialize(EventMeshWriterBufferContext context)
        {

        }
    }
}
