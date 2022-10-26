using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class QueueQueryResult : ISerializable
    {
        public string Vpn { get; set; }
        public string QueueName { get; set; }
        public DateTime? CreateDateTime { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            QueueName = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(QueueName);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.GetValueOrDefault().Ticks));
        }
    }
}
