using FaasNet.CRDT.Client;
using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Core
{
    public class CRDTProtocolHandler : IProtocolHandler
    {
        private readonly ISerializedEntityStore _entityStore;
        private readonly ICRDTEntityFactory _entityFactory;
        private readonly PeerOptions _peerOptions;

        public CRDTProtocolHandler(ISerializedEntityStore entityStore, ICRDTEntityFactory entityFactory, IOptions<PeerOptions> peerOptions)
        {
            _entityStore = entityStore;
            _entityFactory = entityFactory;
            _peerOptions = peerOptions.Value;
        }

        public string MagicCode => CRDTPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var context = new ReadBufferContext(payload);
            CRDTPackage crdtPackage = CRDTPackage.Deserialize(context);
            if (crdtPackage.Type == CRDTPackageTypes.DELTA) return await Handle(crdtPackage as CRDTDeltaPackage, cancellationToken);
            if (crdtPackage.Type == CRDTPackageTypes.SYNC) return await Handle(crdtPackage as CRDTSyncPackage, cancellationToken);
            return CRDTPackageResultBuilder.Ok(crdtPackage.Nonce);
        }

        private async Task<CRDTPackage> Handle(CRDTDeltaPackage deltaPackage, CancellationToken cancellationToken)
        {
            var entity = await _entityStore.Get(deltaPackage.EntityId, cancellationToken);
            if (entity == null) return CRDTPackageResultBuilder.BuildError(deltaPackage, ErrorCodes.UNKNOWN_ENTITY);
            var crdtEntity = _entityFactory.Build(entity);
            crdtEntity.ApplyDelta(_peerOptions.PeerId, deltaPackage.Delta);
            var serializedEntity = new CRDTEntitySerializer().Serialize(entity.Id, crdtEntity);
            await _entityStore.Update(serializedEntity, cancellationToken);
            return CRDTPackageResultBuilder.Ok(deltaPackage.Nonce);
        }

        private async Task<CRDTPackage> Handle(CRDTSyncPackage syncPackage, CancellationToken cancellationToken)
        {
            var entity = await _entityStore.Get(syncPackage.EntityId, cancellationToken);
            if (entity == null) return CRDTPackageResultBuilder.BuildError(syncPackage, ErrorCodes.UNKNOWN_ENTITY);
            var crdtEntity = _entityFactory.Build(entity);
            var diff = new CRDTEntityDiff().Diff(crdtEntity, syncPackage.ClockVector);
            return CRDTPackageResultBuilder.Sync(entity.Id, syncPackage.Nonce, diff);
        }
    }
}
