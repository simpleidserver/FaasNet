using EventMesh.Runtime.Messages;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public class RuntimeClient : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly string _ipAddr;
        private readonly int _port;

        public RuntimeClient(string ipAddr = Constants.DefaultIpAddr, int port = Constants.DefaultPort)
        {
           _udpClient = new UdpClient();
            _ipAddr = ipAddr;
            _port = port;
        }

        public async Task<Package> HeartBeat()
        {
            var writeCtx = new WriteBufferContext();
            PackageRequestBuilder.HeartBeat().Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(IPAddress.Parse(_ipAddr), _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            return Package.Deserialize(readCtx);
        }

        public async Task<Package> Hello(UserAgent userAgent)
        {
            var writeCtx = new WriteBufferContext();
            PackageRequestBuilder.Hello(userAgent).Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(IPAddress.Parse(_ipAddr), _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            return Package.Deserialize(readCtx);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}
