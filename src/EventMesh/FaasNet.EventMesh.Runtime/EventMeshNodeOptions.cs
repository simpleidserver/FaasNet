using System;

namespace FaasNet.EventMesh.Runtime
{
    public class EventMeshNodeOptions
    {
        public EventMeshNodeOptions()
        {
            WaitLocalSubscriptionIntervalMS = 300;
            DefaultSubSessionExpirationTimeSpan = TimeSpan.FromMinutes(60);
            DefaultPubSessionExpirationTimeSpan = TimeSpan.FromMinutes(1);
            MaxPubSessionExpirationTimeSpan = TimeSpan.FromMinutes(5);
            MinSubSessionExpirationTimeSpan = TimeSpan.FromMinutes(1);
        }

        public int WaitLocalSubscriptionIntervalMS { get; set; }
        public TimeSpan DefaultSubSessionExpirationTimeSpan { get; set; }
        public TimeSpan DefaultPubSessionExpirationTimeSpan { get; set; }
        public TimeSpan MaxPubSessionExpirationTimeSpan { get; set; }
        public TimeSpan MinSubSessionExpirationTimeSpan { get; set; }
    }
}
