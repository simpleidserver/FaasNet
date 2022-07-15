using FaasNet.CRDT.Client.Messages.Deltas;
using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTDeltaPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.DELTA;

        public string EntityId { get; set; }
        public BaseEntityDelta Delta { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            Delta.DeltaType.Serialize(context);
            context.WriteString(EntityId);
            Delta.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var deltaType = EntityDeltaTypes.Deserialize(context);
            EntityId = context.NextString();
            if (deltaType == EntityDeltaTypes.GCounter)
            {
                var result = new GCounterDelta();
                result.Extract(context);
                Delta = result;
            }

            if (deltaType == EntityDeltaTypes.GSet)
            {
                var result = new GSetDelta();
                result.Extract(context);
                Delta = result;
            }
        }
    }
}
