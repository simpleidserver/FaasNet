using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllApplicationDomainsResult : BaseEventMeshPackage
    {
        public GetAllApplicationDomainsResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_APPLICATION_DOMAINS_RESULT;
        public GenericSearchQueryResult<ApplicationDomainQueryResult> Content { get; set; } = new GenericSearchQueryResult<ApplicationDomainQueryResult>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            Content.Serialize(context);
        }

        public GetAllApplicationDomainsResult Extract(ReadBufferContext context)
        {
            Content.Deserialize(context);
            return this;
        }
    }
}
