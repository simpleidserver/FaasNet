using EventMesh.Runtime.Events;
using EventMesh.Runtime.Extensions;
using EventMesh.Runtime.Handlers;
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
    public class EventMeshRuntimeHost: IEventMeshRuntimeHost
    {
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private UdpClient _udpClient;
        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        public EventMeshRuntimeHost(IEnumerable<IMessageHandler> messageHandlers)
        {
            _messageHandlers = messageHandlers;
            IsRunning = true;
        }

        public bool IsRunning { get; private set; }
        public event EventHandler<EventArgs> EventMeshRuntimeStarted;
        public event EventHandler<EventMeshPackageEventArgs> EventMeshPackageReceived;
        public event EventHandler<EventMeshPackageEventArgs> EventMeshPackageSent;
        public event EventHandler<EventArgs> EventMeshRuntimeStopped;

        public void Run(string ipAddr = EventMeshRuntimeConstants.DefaultIpAddr, int port = EventMeshRuntimeConstants.DefaultPort)
        {
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(ipAddr), port));
            IsRunning = true;
            Task.Run(async () => await InternalRun());
        }

        public void Stop()
        {
            IsRunning = false;
            _tokenSource.Cancel();
        }

        private async Task InternalRun()
        {
            if (EventMeshRuntimeStarted != null)
            {
                EventMeshRuntimeStarted(this, new EventArgs());
            }

            try
            {
                while(true)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    await HandleEventMeshPackage();
                }
            }
            catch (OperationCanceledException)
            {
                if (EventMeshRuntimeStopped != null)
                {
                    EventMeshRuntimeStopped(this, new EventArgs());
                }

                _udpClient.Close();
            }
        }

        private async Task HandleEventMeshPackage()
        {

            UdpReceiveResult receiveResult;
            try
            {
                receiveResult = await _udpClient.ReceiveAsync().WithCancellation(_cancellationToken);
            }
            catch (SocketException)
            {
                return;
            }

            var buffer = receiveResult.Buffer;
            var package = EventMeshPackage.Deserialize(new EventMeshReaderBufferContext(buffer));

            if (EventMeshPackageReceived != null)
            {
                EventMeshPackageReceived(this, new EventMeshPackageEventArgs(package));
            }

            var messageHandler = _messageHandlers.First(m => m.Command == package.Header.Command);
            var result = await messageHandler.Run(package, receiveResult.RemoteEndPoint, _cancellationToken);
            var writeCtx = new EventMeshWriterBufferContext();
            result.Serialize(writeCtx);
            var resultPayload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(resultPayload, resultPayload.Count(), receiveResult.RemoteEndPoint).WithCancellation(_cancellationToken);
            if(EventMeshPackageSent != null)
            {
                EventMeshPackageSent(this, new EventMeshPackageEventArgs(result));
            }
        }
    }
}
