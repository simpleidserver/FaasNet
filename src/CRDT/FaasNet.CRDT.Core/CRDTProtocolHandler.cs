using FaasNet.CRDT.Client;
using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Core
{
    public class CRDTProtocolHandler : IProtocolHandler<CRDTPackage>
    {
        private readonly IEntityStore _entityStore;
        private readonly IEntityFactory _entityFactory;

        public CRDTProtocolHandler(IEntityStore entityStore, IEntityFactory entityFactory)
        {
            _entityStore = entityStore;
            _entityFactory = entityFactory;
        }

        public async Task<CRDTPackage> Handle(CRDTPackage package, CancellationToken cancellationToken)
        {
            var entity = await _entityStore.Get(package.EntityId, cancellationToken);
            if (entity == null) return CRDTPackageResultBuilder.BuildError(package, ErrorCodes.UNKNOWN_ENTITY);
            if (package.Type == CRDTPackageTypes.DELTA) Handle(entity, package as CRDTDeltaPackage);
            return null;
        }

        private void Handle(SerializedEntity entity, CRDTDeltaPackage detlaPackage)
        {
            _entityFactory.ApplyDelta(entity, detlaPackage.Delta);
        }
    }
}
