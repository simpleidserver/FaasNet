namespace FaasNet.EventMesh.Client.Messages
{
    public class AddClientResponse: Package
    {
        public string Queue { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Queue);
        }

        public void Extract(ReadBufferContext context)
        {
            Queue = context.NextString();
        }
    }
}