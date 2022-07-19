using FaasNet.DHT.Chord.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public interface IRequestHandler
    {
        ChordCommandTypes Command { get; }
        Task<ChordPackage> Handle(ChordPackage request, CancellationToken token);
    }
}
