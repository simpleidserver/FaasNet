using FaasNet.Peer.Client;
using System.Collections.Generic;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTSyncPackage : CRDTPackage
    {
        public CRDTSyncPackage()
        {
            ClockVector = new List<ClockValue>();
        }

        public override CRDTPackageTypes Type => CRDTPackageTypes.SYNC;
        public string PeerId { get; set; }
        public string EntityId { get; set; }
        public ICollection<ClockValue> ClockVector { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PeerId);
            context.WriteString(EntityId);
            context.WriteInteger(ClockVector.Count);
            foreach (var value in ClockVector) value.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            PeerId = context.NextString();
            EntityId = context.NextString();
            int nbValues = context.NextInt();
            for (var i = 0; i < nbValues; i++) ClockVector.Add(ClockValue.Deserialize(context));
        }
    }

    public class ClockValue
    {
        public string ReplicationId { get; set; }
        public int Increment { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ReplicationId);
            context.WriteInteger(Increment);
        }

        public static ClockValue Deserialize(ReadBufferContext context)
        {
            return new ClockValue { ReplicationId = context.NextString(), Increment = context.NextInt() };
        }
    }
}
