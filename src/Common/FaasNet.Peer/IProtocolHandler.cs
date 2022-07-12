using FaasNet.Peer.Client;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public interface IProtocolHandler
    {
        string MagicCode { get; }
        Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken);
    }
}
