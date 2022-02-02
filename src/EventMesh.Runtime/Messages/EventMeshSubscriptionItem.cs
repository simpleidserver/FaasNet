namespace EventMesh.Runtime.Messages
{
    public class EventMeshSubscriptionItem
    {
        public string Topic { get; set; }
        public EventMeshSubscriptionModes Mode { get; set; }
        public EventMeshSubscriptionTypes Type { get; set; }
    }
}
