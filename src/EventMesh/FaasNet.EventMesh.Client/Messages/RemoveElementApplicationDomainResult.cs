using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveElementApplicationDomainResult : BaseEventMeshPackage
    {
        public RemoveElementApplicationDomainResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_ELEMENT_APPLICATION_DOMAIN_RESULT;
        public RemoveElementApplicationDomainStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public RemoveElementApplicationDomainResult Extract(ReadBufferContext context)
        {
            Status = (RemoveElementApplicationDomainStatus)context.NextInt();
            return this;
        }
    }

    public enum RemoveElementApplicationDomainStatus
    {
        OK = 0,
        UNKNOWN_VPN = 1,
        NOLEADER = 2
    }
}
