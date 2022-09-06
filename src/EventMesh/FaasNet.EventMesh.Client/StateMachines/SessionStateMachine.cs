using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using System;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class SessionStateMachine : IStateMachine
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public ClientPurposeTypes ClientPurpose { get; set; }
        public TimeSpan ExpirationTime { get; set; }
        public string TopicFilter { get; set; }
        public bool IsValid => DateTime.UtcNow > new DateTime(ExpirationTime.Ticks);

        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddSessionCommand addSession:
                    ClientId = addSession.ClientId;
                    ClientPurpose = addSession.ClientPurpose;
                    ExpirationTime = addSession.ExpirationTime;
                    TopicFilter = addSession.TopicFilter;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientId = context.NextString();
            ClientPurpose = (ClientPurposeTypes)context.NextInt();
            ExpirationTime = context.NextTimeSpan().Value;
            TopicFilter = context.NextString();
        }

        public byte[] Serialize()
        {
            var result = new WriteBufferContext();
            result.WriteString(Id)
                .WriteString(ClientId)
                .WriteInteger((int)ClientPurpose)
                .WriteTimeSpan(ExpirationTime)
                .WriteString(TopicFilter);
            return result.Buffer.ToArray();
        }
    }

    public class AddSessionCommand : ICommand
    {
        public string ClientId { get; set; }
        public ClientPurposeTypes ClientPurpose { get; set; }
        public string TopicFilter { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            ClientPurpose = (ClientPurposeTypes)context.NextInt();
            TopicFilter = context.NextString();
            ExpirationTime = context.NextTimeSpan().Value;
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId).WriteString(TopicFilter).WriteInteger((int)ClientPurpose).WriteTimeSpan(ExpirationTime);
        }
    }
}
