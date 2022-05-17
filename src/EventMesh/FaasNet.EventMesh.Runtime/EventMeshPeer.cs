using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public class EventMeshPeer : BasePeerHost
    {
        private readonly IMessageExchangeStore _messageExchangeStore;
        private readonly IQueueStore _queueStore;

        public EventMeshPeer(IMessageExchangeStore messageExchangeStore, IQueueStore queueStore, ILogger<BasePeerHost> logger, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore, ILogStore logStore, IPeerInfoStore peerStore) : base(logger, options, clusterStore, logStore, peerStore)
        {
            _messageExchangeStore = messageExchangeStore;
            _queueStore = queueStore;
        }

        protected override Task<bool> HandlePackage(UdpReceiveResult udpResult)
        {
            return Task.FromResult(true);
        }

        protected override Task Init(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override async Task AddEntry(LogRecord logRecord, bool forceAdd, CancellationToken cancellationToken)
        {
            if (State != PeerStates.LEADER && !forceAdd) return;
            var queueNames = await GetQueueNames(cancellationToken);
            await AppendMessage(queueNames, logRecord, cancellationToken);
        }

        private async Task<IEnumerable<string>> GetQueueNames(CancellationToken cancellationToken)
        {
            var messageExchanges = await _messageExchangeStore.GetAll(cancellationToken);
            var queueNames = new List<string>();
            foreach(var messageExchange in messageExchanges)
            {
                if(messageExchange.IsMatch(Info.TermId)) queueNames.AddRange(messageExchange.QueueNames);
            }

            return queueNames.Distinct();
        }

        private async Task AppendMessage(IEnumerable<string> queueNames, LogRecord logRecord, CancellationToken cancellationToken)
        {
            foreach(var queueName in queueNames)
            {
                await _queueStore.Add(queueName, logRecord.Value, cancellationToken);
            }
        }
    }
}
