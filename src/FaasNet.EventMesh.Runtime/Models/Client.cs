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

        private Client(string clientId, string urn)
        {
            ClientId = clientId;
            Urn = urn;
            Topics = new List<ClientTopic>();
            Sessions = new List<ClientSession>();
        }

        #region Properties

        public string ClientId { get; set; }
        public string Urn { get; set; }
        public DateTime CreateDateTime { get; set; }
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

        public string AddSession(IPEndPoint endpoint, string env, int pid, UserAgentPurpose purpose, int bufferCloudEvents, bool isServer)
        {
            var session = ClientSession.Create(endpoint, env, pid, purpose, bufferCloudEvents, isServer ? ClientSessionTypes.SERVER : ClientSessionTypes.CLIENT);
            session.Activate();
            Sessions.Add(session);
            return session.Id;
        }

        public void CloseActiveSession(string sessionId)
        {
            GetActiveSession(sessionId).Close();
        }

        #endregion

        public static Client Create(string clientId, string urn)
        {
            return new Client(clientId, urn)
            {
                CreateDateTime = DateTime.UtcNow
            };
        }
    }
}
