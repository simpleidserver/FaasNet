using CloudNative.CloudEvents;
using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EventMesh.Runtime.Models
{
    public class ClientSession
    {
        private ClientSession()
        {
            Histories = new List<ClientSessionHistory>();
            Topics = new List<Topic>();
            PendingCloudEvents = new List<ClientSessionPendingCloudEvent>();
        }

        #region Properties

        public IPEndPoint Endpoint { get; set; }
        public string Environment { get; set; }
        public int Pid { get; set; }
        public UserAgentPurpose Purpose { get; set; }
        public string Seq { get; set; }
        public int BufferCloudEvents { get; set; }
        public ClientSessionState State { get; set; }
        public ICollection<ClientSessionHistory> Histories { get; set; }
        public ICollection<Topic> Topics { get; set; }
        public ICollection<ClientSessionPendingCloudEvent> PendingCloudEvents { get; set; }

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

        public void UnsubscribeTopic(string topicName, string brokerName)
        {
            var topic = Topics.First(t => t.Name == topicName && t.BrokerName == brokerName);
            Topics.Remove(topic);
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

        #endregion

        public static ClientSession Create(IPEndPoint edp, string env, int pid, string seq, UserAgentPurpose purpose, int bufferCloudEvents)
        {
            var result = new ClientSession
            {
                Endpoint = edp,
                Environment = env,
                Pid = pid,
                Seq = seq,
                Purpose = purpose,
                BufferCloudEvents = bufferCloudEvents
            };
            return result;
        }
    }
}
