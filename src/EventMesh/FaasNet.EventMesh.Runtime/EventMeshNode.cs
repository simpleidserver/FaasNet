using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Handlers;
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
    public class EventMeshNode : BaseNodeHost
    {
        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        public EventMeshNode(IEnumerable<IMessageHandler> messageHandlers, IPeerStore peerStore, RaftConsensus.Core.Stores.INodeStateStore nodeStateStore, IPeerHostFactory peerHostFactory, ILogger<BaseNodeHost> logger, IOptions<ConsensusPeerOptions> options, IEnumerable<RaftConsensus.Core.INodeStateStore> requestHandlers) : base(peerStore, peerHostFactory, logger, options, requestHandlers)
        {
            _messageHandlers = messageHandlers;
        }

        protected override async Task HandleUDPPackage(UdpReceiveResult udpResult, CancellationToken cancellationToken)
        {
            var package = Package.Deserialize(new ReadBufferContext(udpResult.Buffer.ToArray()));
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity(package.Header.Command.Name))
            {
                try
                {
                    EventMeshMeter.IncrementNbIncomingRequest();
                    Logger.LogInformation("Command {command} is received with sequence {sequence}", package.Header.Command.Name, package.Header.Seq);
                    var cmd = package.Header.Command;
                    var messageHandler = _messageHandlers.First(m => m.Command == package.Header.Command);
                    Package result = null;
                    try
                    {
                        result = await messageHandler.Run(package, cancellationToken);
                    }
                    catch (RuntimeException ex)
                    {
                        Logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                        result = PackageResponseBuilder.Error(ex.SourceCommand, ex.SourceSeq, ex.Error);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                        result = PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.INTERNAL_ERROR);
                    }

                    if (result == null)
                    {
                        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                        return;
                    }

                    EventMeshMeter.IncrementNbOutgoingRequest();
                    Logger.LogInformation("Command {command} with sequence {sequence} is going to be sent", result.Header.Command.Name, result.Header.Seq);
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
        }

        // Add state.
    }
}