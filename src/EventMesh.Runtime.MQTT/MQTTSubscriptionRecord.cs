namespace EventMesh.Runtime.MQTT
{
    public class MQTTSubscriptionRecord
    {
        public MQTTSubscriptionRecord(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; set; }
    }
}
