using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class SearchQueuesQueryResult : IQueryResult
    {
        public ICollection<QueueQueryResult> Queues { get; set; } = new List<QueueQueryResult>();

        public void Deserialize(ReadBufferContext context)
        {
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var queue = new QueueQueryResult();
                queue.Deserialize(context);
                Queues.Add(queue);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Queues.Count);
            foreach (var queue in Queues) queue.Serialize(context);
        }
    }
}
