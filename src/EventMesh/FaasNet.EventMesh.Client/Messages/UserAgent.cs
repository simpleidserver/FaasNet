
using FaasNet.RaftConsensus.Client.Messages;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UserAgent
    {
        public UserAgent()
        {
            Purpose = UserAgentPurpose.SUB;
            IsServer = false;
        }

        #region Properties

        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public string Urn { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string Version { get; set; }
        public UserAgentPurpose Purpose { get; set; }
        public bool IsServer { get; set; }
        public int Pid { get; set; }
        public bool IsSessionInfinite { get; set; }
        public TimeSpan? Expiration { get; set; }

        #endregion

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(ClientId);
            context.WriteString(Urn);
            context.WriteInteger(Port);
            context.WriteString(Password);
            context.WriteString(Version);
            Purpose.Serialize(context);
            context.WriteBoolean(IsServer);
            context.WriteInteger(Pid);
            context.WriteBoolean(IsSessionInfinite);
            context.WriteTimeSpan(Expiration);
        }

        public static UserAgent Deserialize(ReadBufferContext context)
        {
            var result = new UserAgent
            {
                Vpn = context.NextString(),
                ClientId = context.NextString(),
                Urn = context.NextString(),
                Port = context.NextInt(),
                Password = context.NextString(),
                Version = context.NextString(),
                Purpose = UserAgentPurpose.Deserialize(context),
                IsServer = context.NextBoolean(),
                Pid = context.NextInt(),
                IsSessionInfinite = context.NextBoolean(),
                Expiration = context.NextTimeSpan()
            };
            return result;
        }

        public override string ToString()
        {
            return $"ClientId = {ClientId}, PID = {Pid}, Purpose = {Purpose}";
        }
    }
}
