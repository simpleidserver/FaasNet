namespace FaasNet.EventMesh.Runtime
{
    public class RuntimeOptions
    {
        public RuntimeOptions()
        {
            Urn = "first.eventmesh.io";
            IPAddress = Client.Constants.DefaultIPAddress;
            Port = Client.Constants.DefaultPort;
            WaitLocalSubscriptionIntervalMS = 300;
        }

        public string Urn { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int WaitLocalSubscriptionIntervalMS { get; set; }
    }
}
