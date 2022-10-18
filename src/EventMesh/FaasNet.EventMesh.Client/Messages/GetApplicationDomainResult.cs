using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetApplicationDomainResult : BaseEventMeshPackage
    {
        public GetApplicationDomainResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_APPLICATION_DOMAIN_RESULT;
        public bool Success { get; set; }
        public ApplicationDomainQueryResult Content { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success) Content.Serialize(context);
        }

        public GetApplicationDomainResult Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                Content = new ApplicationDomainQueryResult();
                Content.Deserialize(context);
            }

            return this;
        }
    }
}
