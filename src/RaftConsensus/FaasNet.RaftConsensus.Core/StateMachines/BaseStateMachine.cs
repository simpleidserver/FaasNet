using FaasNet.Partition;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Infos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public abstract class BaseStateMachine : IStateMachine
    {
        private readonly PeerInfo _peerInfo;
        private readonly IMediator _mediator;

        public BaseStateMachine(IPeerInfoStore peerInfoStore, IMediator mediator)
        {
            _peerInfo = peerInfoStore.Get();
            _mediator = mediator;
        }

        public abstract Task Apply(ICommand cmd, CancellationToken cancellationToken);
        public abstract Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken);
        public abstract Task Commit(CancellationToken cancellationToken);
        public abstract Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken);
        public abstract IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords);
        public abstract Task Truncate(CancellationToken cancellationToken);

        protected async Task PropagateIntegrationEvent<T>(T evt, CancellationToken cancellationToken) where T : class
        {
            if (_peerInfo.Status != PeerStatus.LEADER) return;
            await _mediator.Send(evt, cancellationToken);
        }
    }
}
