using EventMesh.Runtime.Extensions;
using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

        public UdpClient UdpClient
        {
            get
            {
                return _udpClient;
            }
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

        public async Task<Package> Subscribe(ICollection<SubscriptionItem> subscriptionItems, Action<AsyncMessageToClient> callback)
        {
            var writeCtx = new WriteBufferContext();
            PackageRequestBuilder.Subscribe(subscriptionItems).Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(IPAddress.Parse(_ipAddr), _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var listener = new MessageCallbackListener(_udpClient, callback, _ipAddr, _port);
            listener.Listen(CancellationToken.None);
            return Package.Deserialize(readCtx);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }

    public class MessageCallbackListener
    {
        private readonly UdpClient _udpClient;
        private readonly Action<AsyncMessageToClient> _callback;
        private readonly string _ipAddr;
        private readonly int _port;

        public MessageCallbackListener(
            UdpClient udpClient, 
            Action<AsyncMessageToClient> callback,
            string ipAddr,
            int port)
        {
            _udpClient = udpClient;
            _callback = callback;
            _ipAddr = ipAddr;
            _port = port;
        }

        public void Listen(CancellationToken cancellationToken)
        {
            Task.Run(async () => await Run(cancellationToken));
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while(true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                UdpReceiveResult receiveResult;
                try
                {
                    receiveResult = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                }
                catch (SocketException)
                {
                    return;
                }

                var buffer = receiveResult.Buffer;
                var package = Package.Deserialize(new ReadBufferContext(buffer));
                if (package.Header.Command == Commands.ASYNC_MESSAGE_TO_CLIENT)
                {
                    var asyncMessage = package as AsyncMessageToClient;
                    _callback(asyncMessage);
                    var writeCtx = new WriteBufferContext();
                    PackageRequestBuilder.AsyncMessageAckToServer(asyncMessage.BrokerName, asyncMessage.Topic, asyncMessage.CloudEvents.Count(), asyncMessage.Header.Seq).Serialize(writeCtx);
                    var payload = writeCtx.Buffer.ToArray();
                    await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(IPAddress.Parse(_ipAddr), _port));
                }
            }
        }
    }
}
