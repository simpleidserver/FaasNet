using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using System;
using System.Text.Json;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ClientSession
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public int Port { get; set; }
        public string Environment { get; set; }
        public int Pid { get; set; }
        public UserAgentPurpose Purpose
        {
            get
            {
                return new UserAgentPurpose(PurposeCode);
            }
            set
            {
                PurposeCode = value.Code;
            }
        }
        public int PurposeCode { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public int BufferCloudEvents { get; set; }
        public ClientSessionTypes Type { get; set; }
        public ClientSessionState State { get; set; }
        public bool IsActive
        {
            get
            {
                return State == ClientSessionState.ACTIVE && ((ExpirationDateTime == null) || (ExpirationDateTime != null && ExpirationDateTime > DateTime.UtcNow));
            }
        }

        public void Activate()
        {
            State = ClientSessionState.ACTIVE;
        }

        public void Close()
        {
            State = ClientSessionState.FINISH;
        }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.ClientSession,
                EntityId = Id,
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }

        public static ClientSession Create(string env, int pid, UserAgentPurpose purpose, int bufferCloudEvents, string clientId, string vpn, ClientSessionTypes type, TimeSpan expirationTimeSpan, bool isSessionInfinite)
        {
            var createDateTime = DateTime.UtcNow;
            var expirationDateTime = createDateTime.Add(expirationTimeSpan);
            var result = new ClientSession
            {
                Id = Guid.NewGuid().ToString(),
                Environment = env,
                Vpn = vpn,
                ClientId = clientId,
                Pid = pid,
                Purpose = purpose,
                BufferCloudEvents = bufferCloudEvents,
                Type = type,
                CreateDateTime = DateTime.UtcNow,
                ExpirationDateTime = isSessionInfinite ? null : expirationDateTime
            };
            return result;
        }
    }
}
