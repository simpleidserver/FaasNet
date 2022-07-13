using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageResultBuilder
    {
        public static CRDTPackage Ok(string nonce)
        {
            return new CRDTResultPackage { Nonce = nonce };
        }

        public static CRDTPackage Sync(string nonce, ICollection<BaseEntityDelta> deltaLst)
        {
            return new CRDTSyncResultPackage { Nonce = nonce, DiffLst = deltaLst.Select(d =>new CRDTDeltaPackage { Delta = d }).ToList() };
        }

        public static CRDTPackage BuildError(CRDTPackage request, string errorCode)
        {
            return new CRDTErrorPackage { Code = errorCode, Nonce = request.Nonce };
        }
    }
}
