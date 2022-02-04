using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EventMesh.Runtime.Models
{
    public class Client
    {
        private Client() { }

        private Client(string clientId)
        {
            ClientId = clientId;
            Topics = new List<ClientTopic>();
            Sessions = new List<ClientSession>();
        }

        public string ClientId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ClientSession ActiveSession
        {
            get
            {
                return Sessions.FirstOrDefault(s => s.State == ClientSessionState.ACTIVE);
            }
        }
        public ICollection<ClientTopic> Topics { get; set; }
        public ICollection<ClientSession> Sessions { get; set; }

        #region Actions

        public bool HasActiveSession(IPEndPoint edp)
        {
            return ActiveSession.Endpoint.Equals(edp);
        }

        public void AddSession(IPEndPoint edp, string env, int pid, string seq, UserAgentPurpose purpose, int bufferCloudEvents)
        {
            var session = ClientSession.Create(edp, env, pid, seq, purpose, bufferCloudEvents);
            session.Activate();
            Sessions.Add(session);
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

        public void ConsumeCloudEvents(string topicName, string brokerName, int nbEventsConsumed)
        {
            var topic = Topics.First(t => t.Name == topicName && t.BrokerName == brokerName);
            topic.Offset += nbEventsConsumed;
        }

        #endregion

        public static Client Create(string clientId)
        {
            return new Client(clientId)
            {
                CreateDateTime = DateTime.UtcNow
            };
        }
    }
}
