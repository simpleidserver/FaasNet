﻿using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class GetQueueQueryResult : IQueryResult
    {
        public GetQueueQueryResult()
        {
            Success = false;
        }

        public GetQueueQueryResult(QueueQueryResult queue)
        {
            Success = true;
            Queue = queue;
        }

        public bool Success { get; set; }
        public QueueQueryResult Queue { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if(Success)
            {
                Queue = new QueueQueryResult();
                Queue.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success) Queue.Serialize(context);
        }
    }
}