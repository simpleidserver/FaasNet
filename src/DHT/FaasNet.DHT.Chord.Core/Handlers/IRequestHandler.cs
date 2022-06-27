using FaasNet.DHT.Chord.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public interface IRequestHandler
    {
        Commands Command { get; }
        Task<DHTPackage> Handle(DHTPackage request, CancellationToken token);
    }
}
