using FaasNet.EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class Client
    {
        private Client() { }

        private Client(string id, string clientId, string urn)
        {
            Id = id;
            ClientId = clientId;
            Urn = urn;
            Purposes = new List<int>();
            Topics = new List<ClientTopic>();
            Sessions = new List<ClientSession>();
        }

        #region Properties

        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Urn { get; set; }
        public string Vpn { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ICollection<int> Purposes { get; set; }
        public IEnumerable<ClientSession> ActiveSessions
        {
            get
            {
                return Sessions.Where(s => s.State == ClientSessionState.ACTIVE);
            }
        }
        public ICollection<ClientTopic> Topics { get; set; }
        public ICollection<ClientSession> Sessions { get; set; }

        #endregion

        #region Actions

        public bool HasActiveSession(string sessionId)
        {
            return GetActiveSession(sessionId) != null;
        }

        public ClientSession GetActiveSession(string sessionId)
        {
            return ActiveSessions.FirstOrDefault(s => s.Id == sessionId);
        }

        public ClientSession GetActiveSession(string sessionId, string bridgeUrn)
        {
            return ActiveSessions.FirstOrDefault(s => s.Bridges.Any(b => b.Urn == bridgeUrn && b.SessionId == sessionId));
        }

        public ClientSession GetActiveSessionByTopic(string brokerName, string topicName)
        {
            return ActiveSessions.FirstOrDefault(s => s.HasTopic(topicName, brokerName));
        }

        public ClientTopic GetTopic(string topic, string messageBrokerName)
        {
            return Topics.FirstOrDefault(t => t.Name == topic && t.BrokerName == messageBrokerName);
        }

        public ClientTopic AddTopic(string topic, string brokerName)
        {
            var result = new ClientTopic
            {
                BrokerName = brokerName,
                Name = topic
            };
            Topics.Add(result);
            return result;
        }

        public void ConsumeCloudEvents(string brokerName, string topicName, int nbEventsConsumed)
        {
            var topic = Topics.First(t => t.Name == topicName && t.BrokerName == brokerName);
            topic.Offset += nbEventsConsumed;
        }

        public string AddSession(IPEndPoint endpoint, string env, int pid, UserAgentPurpose purpose, int bufferCloudEvents, bool isServer, string vpn)
        {
            var session = ClientSession.Create(endpoint, env, pid, purpose, bufferCloudEvents, vpn, isServer ? ClientSessionTypes.SERVER : ClientSessionTypes.CLIENT);
            session.Activate();
            Sessions.Add(session);
            return session.Id;
        }

        public void CloseActiveSession(string sessionId)
        {
            GetActiveSession(sessionId).Close();
        }

        #endregion

        public static Client Create(string vpn, string clientId, string urn, List<UserAgentPurpose> purposes = null)
        {
            if (purposes == null)
            {
                purposes = new List<UserAgentPurpose>
                {
                    UserAgentPurpose.SUB
                };
            }

            return new Client(Guid.NewGuid().ToString(), clientId, urn)
            {
                CreateDateTime = DateTime.UtcNow,
                Purposes = purposes.Select(p => p.Code).ToList()
            };
        }
    }
}
