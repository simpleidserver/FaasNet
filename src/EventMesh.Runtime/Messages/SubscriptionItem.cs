namespace EventMesh.Runtime.Messages
{
    public class SubscriptionItem
    {
        public string Topic { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Topic);
        }

        public static SubscriptionItem Deserialize(ReadBufferContext context)
        {
            return new SubscriptionItem
            {
                Topic = context.NextString()
            };
        }
    }
}
