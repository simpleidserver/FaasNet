using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ClientSession
    {
        private ClientSession()
        {
            Histories = new List<ClientSessionHistory>();
            Topics = new List<Topic>();
            PendingCloudEvents = new List<ClientSessionPendingCloudEvent>();
            Bridges = new List<ClientSessionBridge>();
        }

        #region Properties

        public IPEndPoint Endpoint
        {
            get
            {
                return new IPEndPoint(new IPAddress(IPAddressData), Port);
            }
            set
            {
                Port = value.Port;
                IPAddressData = value.Address.GetAddressBytes();
            }
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public byte[] IPAddressData { get; set; }
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
        public DateTime ExpirationDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int BufferCloudEvents { get; set; }
        public ClientSessionTypes Type { get; set; }
        public ClientSessionState State { get; set; }
        public ICollection<ClientSessionHistory> Histories { get; set; }
        public ICollection<Topic> Topics { get; set; }
        public ICollection<ClientSessionPendingCloudEvent> PendingCloudEvents { get; set; }
        public ICollection<ClientSessionBridge> Bridges { get; set; }

        #endregion

        #region Actions

        public void Activate()
        {
            Histories.Add(new ClientSessionHistory
            {
                State = ClientSessionState.ACTIVE,
                Timestamp = DateTime.UtcNow
            });
            State = ClientSessionState.ACTIVE;
        }

        public void SubscribeTopic(string topicName, string brokerName)
        {
            Topics.Add(new Topic
            {
                BrokerName = brokerName,
                Name = topicName
            });
        }

        public bool HasTopic(string topicName, string brokerName)
        {
            return Topics.Any(t => t.Name == topicName && t.BrokerName == brokerName);
        }

        public bool TryAddPendingCloudEvent(string brokerName, string topicName, CloudEvent cloudEvt, out ICollection<CloudEvent> cloudEvts)
        {
            cloudEvts = null;
            var pendingCloudEvts = PendingCloudEvents.Where(a => a.BrokerName == brokerName && a.Topic == topicName);
            if (pendingCloudEvts.Count() + 1 >= BufferCloudEvents)
            {
                cloudEvts = pendingCloudEvts.Select(p => p.Evt).ToList();
                cloudEvts.Add(cloudEvt);
                PendingCloudEvents = PendingCloudEvents.Where(a => a.BrokerName != brokerName || a.Topic != topicName).ToList();
                return false;
            }

            PendingCloudEvents.Add(new ClientSessionPendingCloudEvent { BrokerName = brokerName, Topic = topicName, Evt = cloudEvt });
            return true;
        }

        public ClientSessionBridge GetBridge(string urn, int port, string vpn)
        {
            return Bridges.FirstOrDefault(b => b.Urn == urn && b.Port == port && b.Vpn == vpn);
        }

        public void AddBridge(ClientSessionBridge bridge)
        {
            Bridges.Add(bridge);
        }

        public void Close()
        {
            State = ClientSessionState.FINISH;
            Histories.Add(new ClientSessionHistory {  State = ClientSessionState.FINISH, Timestamp = DateTime.UtcNow });
        }

        #endregion

        public static ClientSession Create(IPEndPoint edp, string env, int pid, UserAgentPurpose purpose, int bufferCloudEvents, string vpn, ClientSessionTypes type)
        {
            var result = new ClientSession
            {
                Id = Guid.NewGuid().ToString(),
                Endpoint = edp,
                Environment = env,
                Vpn = vpn,
                Pid = pid,
                Purpose = purpose,
                BufferCloudEvents = bufferCloudEvents,
                Type = type,
                CreateDateTime = DateTime.UtcNow
            };
            return result;
        }
    }
}
