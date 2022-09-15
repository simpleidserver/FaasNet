using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public interface ICommitHelper
    {
        Task Commit(long offset, CancellationToken cancellationToken);
    }

    public class CommitHelper : ICommitHelper
    {
        private readonly RaftConsensusPeerOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogStore _logStore;
        private readonly PeerState _peerState;

        public CommitHelper(IOptions<RaftConsensusPeerOptions> options, IServiceProvider serviceProvider, ILogStore logStore)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _logStore = logStore;
            _peerState = PeerState.New(_options.ConfigurationDirectoryPath, _options.IsConfigurationStoredInMemory);
        }

        public async Task Commit(long offset, CancellationToken cancellationToken)
        {
            var stateMachine = (IStateMachine)ActivatorUtilities.CreateInstance(_serviceProvider, _options.StateMachineType);
            var logs = await _logStore.GetFromTo(_peerState.CommitIndex, offset, cancellationToken);
            foreach (var log in logs)
            {
                var cmd = CommandSerializer.Deserialize(log.Command);
                await stateMachine.Apply(cmd, cancellationToken);
            }

            await stateMachine.Commit(cancellationToken);
            _peerState.CommitIndex = offset;
        }
    }
}