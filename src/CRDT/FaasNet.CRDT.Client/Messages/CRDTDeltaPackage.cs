using FaasNet.CRDT.Client.Messages.Deltas;
using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTDeltaPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.DELTA;

        public BaseEntityDelta Delta { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            Delta.DeltaType.Serialize(context);
            Delta.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var deltaType = EntityDeltaTypes.Deserialize(context);
            if (deltaType == EntityDeltaTypes.GCounter)
            {
                var result = new GCounterDelta();
                result.Extract(context);
                Delta = result;
            }
        }
    }
}
