namespace EventMesh.Runtime.Messages
{
    public class UserAgent
    {
        public UserAgent()
        {
            Purpose = UserAgentPurpose.SUB;
            BufferCloudEvents = 1;
        }

        public string ClientId { get; set; }
        public string Environment { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public int BufferCloudEvents { get; set; }
        public UserAgentPurpose Purpose { get; set; }
        public int Pid { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Environment);
            context.WriteString(Username);
            context.WriteString(Password);
            context.WriteString(Version);
            context.WriteInteger(BufferCloudEvents);
            Purpose.Serialize(context);
            context.WriteInteger(Pid);
        }

        public static UserAgent Deserialize(ReadBufferContext context)
        {
            var result = new UserAgent
            {
                ClientId = context.NextString(),
                Environment = context.NextString(),
                Username = context.NextString(),
                Password = context.NextString(),
                Version = context.NextString(),
                BufferCloudEvents = context.NextInt(),
                Purpose = UserAgentPurpose.Deserialize(context),
                Pid = context.NextInt()
            };
            return result;
        }
    }
}
