using EventMesh.Runtime.Messages;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public class EventMeshRuntimeClient : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly string _ipAddr;
        private readonly int _port;

        public EventMeshRuntimeClient(string ipAddr = EventMeshRuntimeConstants.DefaultIpAddr, int port = EventMeshRuntimeConstants.DefaultPort)
        {
           _udpClient = new UdpClient();
            _ipAddr = ipAddr;
            _port = port;
        }

        public async Task<EventMeshPackage> HeartBeat()
        {
            var writeCtx = new EventMeshWriterBufferContext();
            EventMeshMessageRequestBuilder.HeartBeat().Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(IPAddress.Parse(_ipAddr), _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new EventMeshReaderBufferContext(resultPayload.Buffer);
            return EventMeshPackage.Deserialize(readCtx);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}
