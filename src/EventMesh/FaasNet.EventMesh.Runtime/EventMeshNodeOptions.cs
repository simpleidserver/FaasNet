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
            ProtocolsPluginSubPath = Constants.ProtocolsPluginSubPath;
            SinksPluginSubPath = Constants.SinksPluginSubPath;
        }

        public int WaitLocalSubscriptionIntervalMS { get; set; }
        public TimeSpan DefaultSubSessionExpirationTimeSpan { get; set; }
        public TimeSpan DefaultPubSessionExpirationTimeSpan { get; set; }
        public TimeSpan MaxPubSessionExpirationTimeSpan { get; set; }
        public TimeSpan MinSubSessionExpirationTimeSpan { get; set; }
        /// <summary>
        /// Sub path where protocols plugin are stored.
        /// </summary>
        public string ProtocolsPluginSubPath { get; set; }
        /// <summary>
        /// Sub path where sinks plugin are stored.
        /// </summary>
        public string SinksPluginSubPath { get; set; }
    }
}
