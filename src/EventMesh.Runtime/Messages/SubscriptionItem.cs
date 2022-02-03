namespace EventMesh.Runtime.Messages
{
    public class SubscriptionItem
    {
        public string Topic { get; set; }
        public SubscriptionModes Mode { get; set; }
        public SubscriptionTypes Type { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Topic);
            Mode.Serialize(context);
            Type.Serialize(context);
        }

        public static SubscriptionItem Deserialize(ReadBufferContext context)
        {
            return new SubscriptionItem
            {
                Topic = context.NextString(),
                Mode = SubscriptionModes.Deserialize(context),
                Type = SubscriptionTypes.Deserialize(context)
            };
        }
    }
}
