namespace EventMesh.Runtime.Messages
{
    public class EventMeshSubscriptionTypes
    {
        /// <summary>
        /// SYNC.
        /// </summary>
        public static EventMeshSubscriptionTypes SYNC = new EventMeshSubscriptionTypes("SYNC");
        /// <summary>
        /// ASYNC.
        /// </summary>
        public static EventMeshSubscriptionTypes ASYNC = new EventMeshSubscriptionTypes("ASYNC");

        private EventMeshSubscriptionTypes(string type)
        {
            Type = type;
        }

        public string Type { get; private set; }
    }
}
