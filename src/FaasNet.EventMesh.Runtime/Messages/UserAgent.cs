namespace FaasNet.EventMesh.Runtime.Messages
{
    public class UserAgent
    {
        public UserAgent()
        {
            Purpose = UserAgentPurpose.SUB;
            BufferCloudEvents = 1;
            IsServer = false;
        }

        #region Properties

        public string ClientId { get; set; }
        public string Environment { get; set; }
        public string Urn { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public int BufferCloudEvents { get; set; }
        public UserAgentPurpose Purpose { get; set; }
        public bool IsServer { get; set; }
        public int Pid { get; set; }

        #endregion

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Environment);
            context.WriteString(Urn);
            context.WriteInteger(Port);
            context.WriteString(Password);
            context.WriteString(Version);
            context.WriteInteger(BufferCloudEvents);
            Purpose.Serialize(context);
            context.WriteBoolean(IsServer);
            context.WriteInteger(Pid);
        }

        public static UserAgent Deserialize(ReadBufferContext context)
        {
            var result = new UserAgent
            {
                ClientId = context.NextString(),
                Environment = context.NextString(),
                Urn = context.NextString(),
                Port = context.NextInt(),
                Password = context.NextString(),
                Version = context.NextString(),
                BufferCloudEvents = context.NextInt(),
                Purpose = UserAgentPurpose.Deserialize(context),
                IsServer = context.NextBoolean(),
                Pid = context.NextInt()
            };
            return result;
        }
    }
}
