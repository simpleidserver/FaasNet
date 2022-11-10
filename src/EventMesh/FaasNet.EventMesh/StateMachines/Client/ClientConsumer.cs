using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.StateMachines.ApplicationDomain;
using FaasNet.Partition;
using FaasNet.RaftConsensus.Client;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Client
{
    public class ClientConsumer : BaseIntegrationEventConsumer, IConsumer<ApplicationDomainLinkAdded>, IConsumer<ApplicationDomainLinkRemoved>
    {
        private readonly EventMeshOptions _options;

        public ClientConsumer(IOptions<EventMeshOptions> options, IPartitionCluster partitionCluster) : base(partitionCluster)
        {
            _options = options.Value;
        }

        public void Dispose()
        {
        }

        public async Task Consume(ApplicationDomainLinkAdded request, CancellationToken cancellationToken)
        {
            var cmds = new List<ICommand>
            {
                new AddTargetCommand
                {
                    ClientId = request.Source,
                    Vpn = request.Vpn,
                    Target = request.Target,
                    EventDefId = request.EventId
                },
                new AddSourceCommand
                {
                    ClientId = request.Target,
                    Vpn = request.Vpn,
                    Source = request.Source,
                    EventDefId = request.EventId
                }
            };
            await Parallel.ForEachAsync(cmds, new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.MaxNbThreads
            }, async (cmd, t) =>
            {
                await Send(PartitionNames.CLIENT_PARTITION_KEY, cmd, CancellationToken.None);
            });
        }

        public async Task Consume(ApplicationDomainLinkRemoved request, CancellationToken cancellationToken)
        {
            var cmds = new List<ICommand>
            {
                new RemoveTargetCommand
                {
                    ClientId = request.Source,
                    Vpn = request.Vpn,
                    Target = request.Target,
                    EventDefId = request.EventId
                },
                new RemoveSourceCommand
                {
                    ClientId = request.Target,
                    Vpn = request.Vpn,
                    Source = request.Source,
                    EventDefId = request.EventId
                }
            };
            await Parallel.ForEachAsync(cmds, new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.MaxNbThreads
            }, async (cmd, t) =>
            {
                await Send(PartitionNames.CLIENT_PARTITION_KEY, cmd, CancellationToken.None);
            });
        }
    }
}
