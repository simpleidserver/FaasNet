using FaasNet.CRDT.Client;
using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.Stores;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Core
{
    public class CRDTProtocolHandler : IProtocolHandler
    {
        private readonly IEntityStore _entityStore;
        private readonly IEntityFactory _entityFactory;

        public CRDTProtocolHandler(IEntityStore entityStore, IEntityFactory entityFactory)
        {
            _entityStore = entityStore;
            _entityFactory = entityFactory;
        }

        public string MagicCode => CRDTPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var context = new ReadBufferContext(payload);
            CRDTPackage crdtPackage = CRDTPackage.Deserialize(context);
            var entity = await _entityStore.Get(crdtPackage.EntityId, cancellationToken);
            if (entity == null) return CRDTPackageResultBuilder.BuildError(crdtPackage, ErrorCodes.UNKNOWN_ENTITY);
            if (crdtPackage.Type == CRDTPackageTypes.DELTA) Handle(entity, crdtPackage as CRDTDeltaPackage, crdtPackage.PeerId);
            return null;
        }

        private void Handle(SerializedEntity entity, CRDTDeltaPackage detlaPackage, string peerId)
        {
            _entityFactory.ApplyDelta(entity, detlaPackage.Delta, peerId);
        }
    }
}
