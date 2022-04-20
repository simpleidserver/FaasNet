namespace FaasNet.EventMesh.Client.Messages
{
    public class HelloRequest : Package
    {
        public UserAgent UserAgent { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            UserAgent.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            UserAgent = UserAgent.Deserialize(context);
        }

        public override string ToString()
        {
            return $"Command = {Header.Command}, UserAgent = {UserAgent}";
        }
    }
}
