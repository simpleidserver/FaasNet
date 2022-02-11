using CloudNative.CloudEvents;
using EventMesh.Runtime.Events;
using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Extensions;
using EventMesh.Runtime.Handlers;
using EventMesh.Runtime.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public class RuntimeHost: IRuntimeHost
    {
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly RuntimeOptions _options;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private UdpClient _udpClient;

        public RuntimeHost(
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IMessageConsumer> messageConsumers,
            IUdpClientServerFactory udpClientFactory,
            IOptions<RuntimeOptions> options)
        {
            _messageHandlers = messageHandlers;
            _messageConsumers = messageConsumers;
            _udpClientFactory = udpClientFactory;
            _options = options.Value;
            IsRunning = true;
        }

        public bool IsRunning { get; private set; }
        public event EventHandler<EventArgs> EventMeshRuntimeStarted;
        public event EventHandler<PackageEventArgs> EventMeshPackageReceived;
        public event EventHandler<PackageEventArgs> EventMeshPackageSent;
        public event EventHandler<EventArgs> EventMeshRuntimeStopped;

        public void Run()
        {
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
            _udpClient = _udpClientFactory.Build();
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

            var cmd = package.Header.Command;
            var messageHandler = _messageHandlers.First(m => m.Command == package.Header.Command);
            Package result = null;
            try
            {
                result = await messageHandler.Run(package, receiveResult.RemoteEndPoint, _cancellationToken);
            }
            catch(RuntimeException ex)
            {
                result = PackageResponseBuilder.Error(ex.SourceCommand, ex.SourceSeq, ex.Error);
            }

            if (result == null)
            {
                return;
            }

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
                messageConsumer.CloudEventReceived += async(s, e) => await HandleCloudEventReceived(s, e);
            }
        }

        private async Task HandleCloudEventReceived(object sender, CloudEventArgs e)
        {
            ICollection<CloudEvent> pendingCloudEvts;
            if (e.ClientSession.TryAddPendingCloudEvent(e.BrokerName, e.Topic, e.Evt, out pendingCloudEvts))
            {
                return;
            }

            var writeCtx = new WriteBufferContext();
            var bridgeServers = new List<AsyncMessageBridgeServer>();
            switch (e.ClientSession.Type)
            {
                case Models.ClientSessionTypes.SERVER:
                    bridgeServers.Add(new AsyncMessageBridgeServer
                    {
                        Port = _options.Port,
                        Urn = _options.Urn
                    });
                    PackageResponseBuilder.AsyncMessageToServer(e.ClientId, bridgeServers, e.BrokerName, e.Topic, pendingCloudEvts, e.ClientSession.Seq).Serialize(writeCtx);
                    break;
                case Models.ClientSessionTypes.CLIENT:
                    PackageResponseBuilder.AsyncMessageToClient(bridgeServers, e.BrokerName, e.Topic, pendingCloudEvts, e.ClientSession.Seq).Serialize(writeCtx);
                    break;
            }

            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), e.ClientSession.Endpoint).WithCancellation(_cancellationToken);
        }

        #endregion
    }
}
