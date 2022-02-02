namespace EventMesh.Runtime.Messages
{
    public class EventMeshSubscriptionModes
    {
        /// <summary>
        /// Broadcast.
        /// </summary>
        public static EventMeshSubscriptionModes BROADCASTING = new EventMeshSubscriptionModes("BROADCASTING");
        /// <summary>
        /// Clustering.
        /// </summary>
        public static EventMeshSubscriptionModes CLUSTERING = new EventMeshSubscriptionModes("CLUSTERING");

        public EventMeshSubscriptionModes(string mode)
        {
            Mode = mode;
        }

        public string Mode { get; private set; }
    }
}
