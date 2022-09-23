using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetClientResult : BaseEventMeshPackage
    {
        public GetClientResult(string seq) : base(seq)
        {
        }

        public GetClientStatus Status { get; set; }
        public ClientQueryResult Content { get; set; }
        public override EventMeshCommands Command => EventMeshCommands.GET_CLIENT_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            if (Status == GetClientStatus.OK) Content.Serialize(context);
        }

        public GetClientResult Extract(ReadBufferContext context)
        {
            Status = (GetClientStatus)context.NextInt();
            if(Status == GetClientStatus.OK)
            {
                Content = new ClientQueryResult();
                Content.Deserialize(context);
            }

            return this;
        }
    }

    public enum GetClientStatus
    {
        OK = 0,
        UNKNOWN = 1
    }
}
