using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public interface IMessageHandler
    {
        Commands Command { get; }
        Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken);
    }
}
