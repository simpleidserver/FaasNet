using FaasNet.EventMesh.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Results
{
    public class ClientSessionResult
    {
        public ClientSessionResult()
        {
            Topics = new List<TopicResult>();
        }

        public int Pid { get; set; }
        public int Purpose { get; set; }
        public int BufferCloudEvents { get; set; }
        public DateTime CreationDateTime { get; set; }
        public ClientSessionState State { get; set; }
        public ClientSessionTypes Type { get; set; }
        public IEnumerable<TopicResult> Topics { get; set; }

        public static ClientSessionResult Build(ClientSession session)
        {
            return new ClientSessionResult
            {
                Pid = session.Pid,
                Purpose = session.Purpose.Code,
                BufferCloudEvents = session.BufferCloudEvents,
                CreationDateTime = session.CreateDateTime,
                State = session.State,
                Type = session.Type,
                Topics = session.Topics.Select(t => TopicResult.Build(t))
            };
        }
    }
}
