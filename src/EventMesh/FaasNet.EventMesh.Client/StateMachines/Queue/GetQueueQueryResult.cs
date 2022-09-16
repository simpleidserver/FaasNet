﻿using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class GetQueueQueryResult : IQueryResult
    {
        public string QueueName { get; set; }
        public string TopicFilter { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            QueueName = context.NextString();
            TopicFilter = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(QueueName);
            context.WriteString(TopicFilter);
        }
    }
}
