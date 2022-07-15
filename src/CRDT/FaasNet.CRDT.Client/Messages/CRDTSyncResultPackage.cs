using FaasNet.CRDT.Client.Messages.Deltas;
using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTSyncResultPackage : CRDTPackage
    {
        public CRDTSyncResultPackage()
        {
            DiffLst = new List<CRDTSyncDiffRecordPackage>();
        }

        public override CRDTPackageTypes Type => CRDTPackageTypes.SYNCRESULT;

        public string PeerId { get; set; }
        public string EntityId { get; set; }
        public ICollection<CRDTSyncDiffRecordPackage> DiffLst { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PeerId);
            context.WriteString(EntityId);
            context.WriteInteger(DiffLst.Count());
            foreach(var diff in DiffLst) diff.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<CRDTSyncDiffRecordPackage>();
            PeerId = context.NextString();
            EntityId = context.NextString();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var pkg = new CRDTSyncDiffRecordPackage();
                pkg.Extract(context);
                result.Add(pkg);
            }

            DiffLst = result;
        }
    }

    public class CRDTSyncDiffRecordPackage
    {
        public string PeerId { get; set; }
        public BaseEntityDelta Delta { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(PeerId);
            Delta.DeltaType.Serialize(context);
            Delta.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            PeerId = context.NextString();
            var deltaType = EntityDeltaTypes.Deserialize(context);
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

            if(deltaType == EntityDeltaTypes.PNCounter)
            {
                var result = new PNCounterDelta();
                result.Extract(context);
                Delta = result;
            }
        }
    }
}
