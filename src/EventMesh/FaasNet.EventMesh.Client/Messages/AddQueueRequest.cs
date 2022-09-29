﻿using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddQueueRequest : BaseEventMeshPackage
    {
        public AddQueueRequest(string seq) : base(seq)
        {
        }
        public AddQueueRequest(string seq, string topic) : base(seq)
        {
            TopicFilter = topic;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_QUEUE_REQUEST;

        public string Vpn { get; set; }
        public string QueueName { get; set; }
        public string TopicFilter { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(QueueName);
            context.WriteString(TopicFilter);
        }

        public AddQueueRequest Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            QueueName = context.NextString();
            TopicFilter = context.NextString();
            return this;
        }
    }
}