using System.Collections.Generic;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageResultBuilder
    {
        public static CRDTPackage Ok(string nonce)
        {
            return new CRDTResultPackage { Nonce = nonce };
        }

        public static CRDTPackage Sync(string entityId, string nonce, ICollection<CRDTSyncDiffRecordPackage> diffLst)
        {
            return new CRDTSyncResultPackage { EntityId = entityId, Nonce = nonce, DiffLst = diffLst };
        }

        public static CRDTPackage BuildError(CRDTPackage request, string errorCode)
        {
            return new CRDTErrorPackage { Code = errorCode, Nonce = request.Nonce };
        }
    }
}
