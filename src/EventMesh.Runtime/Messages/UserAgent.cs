namespace EventMesh.Runtime.Messages
{
    public class UserAgent
    {
        public UserAgent()
        {
            Purpose = UserAgentPurpose.SUB;
        }

        public string Environment { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public UserAgentPurpose Purpose { get; set; }
        public int Pid { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Environment);
            context.WriteString(Username);
            context.WriteString(Password);
            context.WriteString(Version);
            Purpose.Serialize(context);
            context.WriteInteger(Pid);
        }

        public static UserAgent Deserialize(ReadBufferContext context)
        {
            var result = new UserAgent
            {
                Environment = context.NextString(),
                Username = context.NextString(),
                Password = context.NextString(),
                Version = context.NextString(),
                Purpose = UserAgentPurpose.Deserialize(context),
                Pid = context.NextInt()
            };
            return result;
        }
    }
}
