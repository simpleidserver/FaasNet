using System;

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
            DefaultSubSessionExpirationTimeSpan = TimeSpan.FromMinutes(60);
            DefaultPubSessionExpirationTimeSpan = TimeSpan.FromMinutes(1);
            MaxPubSessionExpirationTimeSpan = TimeSpan.FromMinutes(5);
            MinSubSessionExpirationTimeSpan = TimeSpan.FromMinutes(1);
        }

        public string Urn { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int WaitLocalSubscriptionIntervalMS { get; set; }
        public TimeSpan DefaultSubSessionExpirationTimeSpan { get; set; }
        public TimeSpan DefaultPubSessionExpirationTimeSpan { get; set; }
        public TimeSpan MaxPubSessionExpirationTimeSpan { get; set; }
        public TimeSpan MinSubSessionExpirationTimeSpan { get; set; }
    }
}
