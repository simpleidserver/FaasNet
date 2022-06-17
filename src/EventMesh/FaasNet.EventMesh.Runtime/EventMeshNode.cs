using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Handlers;
using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
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

        public EventMeshNode(IEnumerable<IMessageHandler> messageHandlers, IPeerStore peerStore, IPeerInfoStore peerInfoStore, INodeStateStore nodeStateStore, IClusterStore clusterStore, IPeerHostFactory peerHostFactory, ILogger<BaseNodeHost> logger, IOptions<ConsensusNodeOptions> options) : base(peerStore, peerInfoStore, peerHostFactory, nodeStateStore, clusterStore, logger, options)
        {
            _messageHandlers = messageHandlers;
        }

        protected override async Task HandlePackage(UdpReceiveResult udpResult, CancellationToken cancellationToken)
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
                    EventMeshPackageResult result = null;
                    try
                    {
                        result = await messageHandler.Run(package, Peers, cancellationToken);
                    }
                    catch (RuntimeException ex)
                    {
                        Logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                        result = EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(ex.SourceCommand, ex.SourceSeq, ex.Error));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Command {command}, sequence {sequence}, exception {exception}", package.Header.Command.Name, package.Header.Seq, ex.ToString());
                        result = EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.INTERNAL_ERROR));
                    }

                    if (result == null)
                    {
                        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                        return;
                    }

                    if(result.Status.HasFlag(EventMeshPackageResultStatus.ADD_PEER))
                    {
                        await AddPeer(result.Termid);
                        var peerHost = await StartPeer(result.Termid);
                        if (result.LogRecord != null) await peerHost.AppendEntry(result.LogRecord, true, TokenSource.Token);
                    }

                    if(result.Status.HasFlag(EventMeshPackageResultStatus.SEND_RESULT))
                    {
                        EventMeshMeter.IncrementNbOutgoingRequest();
                        Logger.LogInformation("Command {command} with sequence {sequence} is going to be sent", result.Package.Header.Command.Name, result.Package.Header.Seq);
                        var writeCtx = new WriteBufferContext();
                        result.Package.Serialize(writeCtx);
                        var resultPayload = writeCtx.Buffer.ToArray();
                        await UdpServer.SendAsync(resultPayload, resultPayload.Count(), udpResult.RemoteEndPoint).WithCancellation(TokenSource.Token);
                    }

                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                }
                catch (Exception)
                {
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                    throw;
                }
            }
        }
    }
}