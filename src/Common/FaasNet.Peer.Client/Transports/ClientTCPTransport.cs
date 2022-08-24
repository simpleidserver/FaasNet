using FaasNet.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client.Transports
{
    public class ClientTCPTransport : IClientTransport
    {
        private Socket _socket;

        public void Open(string url, int port)
        {
            Open(new IPEndPoint(DnsHelper.ResolveIPV4(url), port));
        }

        public void Open(IPEndPoint edp)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(edp);
        }

        public Task<byte[]> Receive(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buffer = new byte[2048];
            int bytesRead = 0;
            var result = new List<byte>();
            while((bytesRead = _socket.Receive(buffer, 0, timeoutMS)) > 0)
                result.AddRange(buffer.Take(bytesRead));
            return Task.FromResult(result.ToArray());
        }

        public Task<int> Send(byte[] payload, int timeousMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            _socket.Send(payload, 0, timeousMS);
            return Task.FromResult(payload.Count());
        }

        public void Close()
        {
            _socket?.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
