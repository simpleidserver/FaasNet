using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ClientSession
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public int Port { get; set; }
        public int EvtOffset { get; set; }
        public int Pid { get; set; }
        public int PurposeCode { get; set; }
        [JsonIgnore]
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
        public DateTime CreateDateTime { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public ClientSessionTypes Type { get; set; }
        public ClientSessionState State { get; set; }
        public bool IsActive
        {
            get
            {
                return State == ClientSessionState.ACTIVE && ((ExpirationDateTime == null) || (ExpirationDateTime != null && ExpirationDateTime > DateTime.UtcNow));
            }
        }
        public string Queue
        {
            get
            {
                return $"{Vpn}_{ClientId}_{Id}";
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

        public void ConsumeEvent()
        {
            EvtOffset++;
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

        public static ClientSession Create(int pid, UserAgentPurpose purpose, string clientId, string vpn, ClientSessionTypes type, TimeSpan expirationTimeSpan, bool isSessionInfinite)
        {
            var createDateTime = DateTime.UtcNow;
            var expirationDateTime = createDateTime.Add(expirationTimeSpan);
            var result = new ClientSession
            {
                Id = Guid.NewGuid().ToString(),
                Vpn = vpn,
                ClientId = clientId,
                Pid = pid,
                Purpose = purpose,
                Type = type,
                CreateDateTime = DateTime.UtcNow,
                EvtOffset = 1,
                ExpirationDateTime = isSessionInfinite ? null : expirationDateTime
            };
            return result;
        }
    }
    public enum ClientSessionState
    {
        ACTIVE = 0,
        FINISH = 1
    }

    public enum ClientSessionTypes
    {
        CLIENT = 0,
        SERVER = 1
    }
}
