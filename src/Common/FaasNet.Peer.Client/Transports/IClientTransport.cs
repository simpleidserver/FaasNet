using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client.Transports
{
    public interface IClientTransport : IDisposable
    {
        void Open(string url, int port);
        void Open(IPEndPoint edp);
        void Close();
        Task<int> Send(byte[] payload, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> Receive(int timeousMS = 500, CancellationToken cancellationToken = default(CancellationToken));
        IClientTransport CloneAndOpen();
    }
}
