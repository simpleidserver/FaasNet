using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.StateMachines.Session
{
    public class SessionQueryResult
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public ClientPurposeTypes ClientPurpose { get; set; }
        public TimeSpan ExpirationTime { get; set; }
        public string QueueName { get; set; }
        public bool IsValid => DateTime.UtcNow.Ticks < ExpirationTime.Ticks;

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientId = context.NextString();
            ClientPurpose = (ClientPurposeTypes)context.NextInt();
            ExpirationTime = context.NextTimeSpan().Value;
            QueueName = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientId);
            context.WriteInteger((int)ClientPurpose);
            context.WriteTimeSpan(ExpirationTime);
            context.WriteString(QueueName);
        }
    }
}
