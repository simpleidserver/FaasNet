using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllVpnResult : BaseEventMeshPackage
    {
        public GetAllVpnResult(string seq) : base(seq)
        {
        }

        public GenericSearchQueryResult<VpnQueryResult> Content { get; set; } = new GenericSearchQueryResult<VpnQueryResult>();

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_VPN_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            Content.Serialize(context);
        }

        public GetAllVpnResult Extract(ReadBufferContext context)
        {
            Content.Deserialize(context);
            return this;
        }
    }

    public class VpnResult
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Description);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Ticks));
        }

        public static VpnResult Extract(ReadBufferContext context)
        {
            return new VpnResult
            {
                Id = context.NextString(),
                Description = context.NextString(),
                CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks),
                UpdateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks)
            };
        }
    }
}
