namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageResultBuilder
    {
        public static CRDTPackage BuildError(CRDTPackage request, string errorCode)
        {
            return new CRDTErrorPackage { Code = errorCode, Nonce = request.Nonce, EntityId = request.EntityId };
        }
    }
}
