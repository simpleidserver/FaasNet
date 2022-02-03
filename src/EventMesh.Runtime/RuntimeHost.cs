using EventMesh.Runtime.Events;
using EventMesh.Runtime.Extensions;
using EventMesh.Runtime.Handlers;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public class RuntimeHost: IRuntimeHost
    {
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private UdpClient _udpClient;
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;
        private readonly IClientSessionStore _clientSessionStore;

        public RuntimeHost(
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IMessageConsumer> messageConsumers,
            IClientSessionStore clientSessionStore)
        {
            _messageHandlers = messageHandlers;
            _messageConsumers = messageConsumers;
            _clientSessionStore = clientSessionStore;
            IsRunning = true;
        }

        public bool IsRunning { get; private set; }
        public event EventHandler<EventArgs> EventMeshRuntimeStarted;
        public event EventHandler<PackageEventArgs> EventMeshPackageReceived;
        public event EventHandler<PackageEventArgs> EventMeshPackageSent;
        public event EventHandler<EventArgs> EventMeshRuntimeStopped;

        public void Run(string ipAddr = Constants.DefaultIpAddr, int port = Constants.DefaultPort)
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

            await InitMessageConsumers();
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

        #region Handle EventMesh package

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
            var package = Package.Deserialize(new ReadBufferContext(buffer));

            if (EventMeshPackageReceived != null)
            {
                EventMeshPackageReceived(this, new PackageEventArgs(package));
            }

            var messageHandler = _messageHandlers.First(m => m.Command == package.Header.Command);
            var result = await messageHandler.Run(package, receiveResult.RemoteEndPoint, _cancellationToken);
            var writeCtx = new WriteBufferContext();
            result.Serialize(writeCtx);
            var resultPayload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(resultPayload, resultPayload.Count(), receiveResult.RemoteEndPoint).WithCancellation(_cancellationToken);
            if(EventMeshPackageSent != null)
            {
                EventMeshPackageSent(this, new PackageEventArgs(result));
            }
        }

        #endregion

        #region Listen messages

        private async Task InitMessageConsumers()
        {
            foreach (var messageConsumer in _messageConsumers)
            {
                await messageConsumer.Start(_cancellationToken);
                messageConsumer.CloudEventReceived += HandleCloudEventReceived;
            }
        }

        private void HandleCloudEventReceived(object sender, CloudEventArgs e)
        {
            var cloudEvt = e.Evt;
            // GET SESSIONS WHICH HAVE SUBSCRIPTION TO A TOPIC.

        }

        #endregion
    }
}
