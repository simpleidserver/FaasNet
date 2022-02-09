namespace EventMesh.Runtime
{
    public class RuntimeOptions
    {
        public RuntimeOptions()
        {
            Urn = "first.eventmesh.io";
            IPAddress = Constants.DefaultIPAddress;
            Port = Constants.DefaultPort;
        }

        public string Urn { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }
}
