using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Handlers;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public class EventMeshPeer: BasePeerHost
    {
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;
        private readonly IClientStore _clientStore;
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly ILogger<EventMeshPeer> _logger;
        private readonly RuntimeOptions _options;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private UdpClient _udpClient;

        public EventMeshPeer(IEnumerable<IMessageHandler> messageHandlers, ILogger<BasePeerHost> logger, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore, ILogStore logStore, IPeerStore peerStore) : base(logger, options, clusterStore, logStore, peerStore)
        {
            _messageHandlers = messageHandlers;
        }

        protected override async Task<bool> HandlePackage(UdpReceiveResult udpResult)
        {
            var package = Package.Deserialize(new ReadBufferContext(udpResult.Buffer.ToArray()));
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity(package.Header.Command.Name))
            {
                try
                {
                    EventMeshMeter.IncrementNbIncomingRequest();
                    _logger.LogInformation("Command {command} is received with sequence {sequence}", package.Header.Command.Name, package.Header.Seq);
                    var cmd = package.Header.Command;
                    var messageHandler = _messageHandlers.First(m => m.Command == package.Header.Command);
                    Package result = null;
                    try
                    {
                        result = await messageHandler.Run(package, _cancellationToken);
                    }
                    catch (RuntimeException ex)
                    {
                        _logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                        result = PackageResponseBuilder.Error(ex.SourceCommand, ex.SourceSeq, ex.Error);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                        result = PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.INTERNAL_ERROR);
                    }

                    if (result == null)
                    {
                        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                        return false;
                    }

                    EventMeshMeter.IncrementNbOutgoingRequest();
                    _logger.LogInformation("Command {command} with sequence {sequence} is going to be sent", result.Header.Command.Name, result.Header.Seq);
                    var writeCtx = new WriteBufferContext();
                    result.Serialize(writeCtx);
                    var resultPayload = writeCtx.Buffer.ToArray();
                    // await _udpClient.SendAsync(resultPayload, resultPayload.Count(), receiveResult.RemoteEndPoint).WithCancellation(_cancellationToken);
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                }
                catch (Exception)
                {
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                    throw;
                }
            }

            return true;
        }

        /*
        public EventMeshPeer(
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IMessageConsumer> messageConsumers,
            IUdpClientServerFactory udpClientFactory,
            ILogger<EventMeshPeer> logger,
            IClientStore clientStore,
            IOptions<RuntimeOptions> options)
        {
            _messageHandlers = messageHandlers;
            _messageConsumers = messageConsumers;
            _udpClientFactory = udpClientFactory;
            _clientStore = clientStore;
            _logger = logger;
            _options = options.Value;
            IsRunning = true;
            EventMeshMeter.Init();
        }

        public bool IsRunning { get; private set; }
        public event EventHandler<EventArgs> EventMeshRuntimeStarted;
        public event EventHandler<PackageEventArgs> EventMeshPackageReceived;
        public event EventHandler<PackageEventArgs> EventMeshPackageSent;
        public event EventHandler<EventArgs> EventMeshRuntimeStopped;

        public async Task Run(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start Event mesh server");
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
            _udpClient = _udpClientFactory.Build();
            IsRunning = true;
            await _clientStore.CloseAllActiveSessions(cancellationToken);
            await _clientStore.SaveChanges(cancellationToken);
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), CancellationToken.None);
#pragma warning restore 4014
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop Event mesh server");
            await _clientStore.CloseAllActiveSessions(cancellationToken);
            await _clientStore.SaveChanges(cancellationToken);
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
                await InitMessageConsumers();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                IsRunning = false;
                throw;
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
                _logger.LogInformation("Event mesh server is stopped");
                if (EventMeshRuntimeStopped != null)
                {
                    EventMeshRuntimeStopped(this, new EventArgs());
                }

                _udpClient.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        #region Handle EventMesh package

        private async Task HandleEventMeshPackage()
        {
            try
            {
                UdpReceiveResult receiveResult = receiveResult = await _udpClient.ReceiveAsync().WithCancellation(_cancellationToken);
                var buffer = receiveResult.Buffer;
                var package = Package.Deserialize(new ReadBufferContext(buffer));
                if (EventMeshPackageReceived != null)
                {
                    EventMeshPackageReceived(this, new PackageEventArgs(package));
                }

                // La requête heartbeat peut être traitée que par le leader.
                // La requête hello message peut être traitée que par le leader.
                using (var activity = EventMeshMeter.RequestActivitySource.StartActivity(package.Header.Command.Name))
                {
                    try
                    {
                        EventMeshMeter.IncrementNbIncomingRequest();
                        _logger.LogInformation("Command {command} is received with sequence {sequence}", package.Header.Command.Name, package.Header.Seq);
                        var cmd = package.Header.Command;
                        var messageHandler = _messageHandlers.First(m => m.Command == package.Header.Command);
                        Package result = null;
                        try
                        {
                            result = await messageHandler.Run(package, receiveResult.RemoteEndPoint, _cancellationToken);
                        }
                        catch (RuntimeException ex)
                        {
                            _logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                            result = PackageResponseBuilder.Error(ex.SourceCommand, ex.SourceSeq, ex.Error);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                            result = PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.INTERNAL_ERROR);
                        }

                        if (result == null)
                        {
                            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                            return;
                        }

                        EventMeshMeter.IncrementNbOutgoingRequest();
                        _logger.LogInformation("Command {command} with sequence {sequence} is going to be sent", result.Header.Command.Name, result.Header.Seq);
                        var writeCtx = new WriteBufferContext();
                        result.Serialize(writeCtx);
                        var resultPayload = writeCtx.Buffer.ToArray();
                        await _udpClient.SendAsync(resultPayload, resultPayload.Count(), receiveResult.RemoteEndPoint).WithCancellation(_cancellationToken);
                        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                        if (EventMeshPackageSent != null)
                        {
                            EventMeshPackageSent(this, new PackageEventArgs(result));
                        }
                    }
                    catch(Exception)
                    {
                        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                        throw;
                    }
                }
            }
            catch(AggregateException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        #endregion

        #region Listen messages

        private async Task InitMessageConsumers()
        {
            _logger.LogInformation("Initialize message consumers");
            foreach (var messageConsumer in _messageConsumers)
            {
                await messageConsumer.Start(_cancellationToken);
                messageConsumer.CloudEventReceived += async(s, e) => await HandleCloudEventReceived(s, e);
            }
        }

        private async Task HandleCloudEventReceived(object sender, CloudEventArgs e)
        {
            _logger.LogInformation("Event with attributes : id={evtId}, subject={evtSubject}, source={evtSource}, type={evtType} is received from {evtBrokerName}", e.Evt.Id, e.Evt.Subject, e.Evt.Source, e.Evt.Type, e.BrokerName);
            ICollection<CloudEvent> pendingCloudEvts;
            if (e.ClientSession.TryAddPendingCloudEvent(e.BrokerName, e.TopicMessage, e.Evt, out pendingCloudEvts))
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
                        Urn = _options.Urn,
                        Vpn = e.ClientSession.Vpn
                    });
                    PackageResponseBuilder.AsyncMessageToServer(e.ClientId, bridgeServers, e.BrokerName, e.TopicMessage, e.TopicFilter, pendingCloudEvts, e.ClientSession.Id).Serialize(writeCtx);
                    break;
                case Models.ClientSessionTypes.CLIENT:
                    PackageResponseBuilder.AsyncMessageToClient(bridgeServers, e.BrokerName, e.TopicMessage, e.TopicFilter, pendingCloudEvts).Serialize(writeCtx);
                    break;
            }

            var payload = writeCtx.Buffer.ToArray();
            _logger.LogInformation("Event id={evtId} is going to be sent to the client {clientId}", e.Evt.Id, e.ClientId);
            await _udpClient.SendAsync(payload, payload.Count(), e.ClientSession.Endpoint).WithCancellation(_cancellationToken);
        }

        #endregion
        */
    }
}
