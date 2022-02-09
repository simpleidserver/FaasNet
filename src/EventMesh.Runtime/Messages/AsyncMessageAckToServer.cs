namespace EventMesh.Runtime.Messages
{
    public class AsyncMessageAckToServer : Package
    {
        #region Properties

        public int NbCloudEventsConsumed { get; set; }
        public string Topic { get; set; }
        public string BrokerName { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(NbCloudEventsConsumed);
            context.WriteString(Topic);
            context.WriteString(BrokerName);
        }

        public void Extract(ReadBufferContext context)
        {
            NbCloudEventsConsumed = context.NextInt();
            Topic = context.NextString();
            BrokerName = context.NextString();
        }
    }
}
