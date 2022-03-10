namespace FaasNet.EventStore
{
    public class EventStoreOptions
    {
        public EventStoreOptions()
        {
            SnapshotFrequency = 5;
        }

        public int SnapshotFrequency { get; set; }
    }
}
