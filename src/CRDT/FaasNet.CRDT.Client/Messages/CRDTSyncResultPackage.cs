using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTSyncResultPackage : CRDTPackage
    {
        public CRDTSyncResultPackage()
        {
            DiffLst = new List<CRDTDeltaPackage>();
        }

        public override CRDTPackageTypes Type => CRDTPackageTypes.SYNCRESULT;

        public ICollection<CRDTDeltaPackage> DiffLst { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(DiffLst.Count());
            foreach(var diff in DiffLst) diff.SerializeAction(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<CRDTDeltaPackage>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var pkg = new CRDTDeltaPackage();
                pkg.Extract(context);
                result.Add(pkg);
            }
        }
    }
}
