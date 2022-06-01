using Amqp;
using Amqp.Sasl;
using Amqp.Types;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP
{
    public class AMQPProxy : BaseProxy
    {
        private readonly EventMeshAMQPOptions _options;
        private readonly IEnumerable<IRequestHandler> _requestHandlers;
        private static ManualResetEvent _lock = new ManualResetEvent(false);
        private readonly ILogger<AMQPProxy> _logger;
        private Socket _server;

        public AMQPProxy(IOptions<EventMeshAMQPOptions> options, IEnumerable<IRequestHandler> requestHandlers, ILogger<AMQPProxy> logger)
        {
            _options = options.Value;
            _requestHandlers = requestHandlers;
            _logger = logger;
        }

        protected override void Init()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Loopback, _options.Port);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndPoint);
            _server.Listen();
            Task.Run(() => Handle(_server));
        }

        protected override void Shutdown()
        {
            _server.Close();
        }

        private void Handle(Socket server)
        {
            while (true)
            {
                TokenSource.Token.ThrowIfCancellationRequested();
                _lock.Reset();
                server.BeginAccept(new AsyncCallback(AcceptCallback), server);
                _lock.WaitOne();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _lock.Set();
            try
            {
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);
                var state = new StateObject
                {
                    WorkSocket = handler
                };
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async void ReadCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;
            try
            {
                int nbBytes = handler.EndReceive(ar);
                byte[] buffer = state.Buffer.Take(nbBytes).ToArray();
                if (ProtocolHeader.SASLHeaderNegotiation.Serialize().SequenceEqual(buffer))
                {
                    ReplySASHeaderNegotiation(state);
                    return;
                }

                if (ProtocolHeader.SASLHeaderSecuredConnection.Serialize().SequenceEqual(buffer))
                {
                    ReplySASLHeaderSecuredConnection(state);
                    return;
                }

                await ReplyFrame(state, buffer);
            }
            catch (Exception ex)
            {
                state.End();
                _logger.LogError(ex.ToString());
            }
        }

        private void ReplySASHeaderNegotiation(StateObject stateObject)
        {
            var result = ProtocolHeader.SASLHeaderNegotiation.Serialize();
            var handler = stateObject.WorkSocket;
            handler.Send(result, 0, result.Length, 0);
            var saslFrame = new Frame { Channel = 0, Type = FrameTypes.Sasl };
            var cmd = new SaslMechanisms
            {
                SaslServerMechanisms = new Symbol[]
                {
                    new Symbol("PLAIN")
                }
            };
            var buffer = saslFrame.Serialize(cmd);
            handler.BeginSend(buffer.Buffer, buffer.Offset, buffer.Length, 0, new AsyncCallback(SendCallback), stateObject);
        }

        private void ReplySASLHeaderSecuredConnection(StateObject stateObject)
        {
            var result = ProtocolHeader.SASLHeaderSecuredConnection.Serialize();
            var handler = stateObject.WorkSocket;
            handler.Send(result, 0, result.Length, 0);
            var newState = new StateObject { WorkSocket = stateObject.WorkSocket, Session = stateObject.Session };
            newState.WorkSocket.BeginReceive(newState.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), newState);
        }

        private async Task ReplyFrame(StateObject stateObject, byte[] buffer)
        {
            ushort channel;
            DescribedList command;
            var handler = stateObject.WorkSocket;
            var frameBuffer = new ByteBuffer(buffer, 0, buffer.Count(), 0);
            byte[] payload = null;
            Frame.Decode(frameBuffer, out channel, out command);
            if (frameBuffer.Length > 0) payload = buffer.Skip(frameBuffer.Offset + FixedWidth.UInt).Take(frameBuffer.Length - FixedWidth.UInt).ToArray();
            _logger.LogInformation("Receive {command}", command.Descriptor.Name);
            var requestHandler = _requestHandlers.First(r => r.RequestName == command.Descriptor.Name);
            var parameter = new RequestParameter(command, payload, channel);
            var result = await requestHandler.Handle(stateObject, parameter, TokenSource.Token);
            if (result.Status == RequestResultStatus.EXIT_SESSION)
            {   if(result.Content != null && result.Content.Any()) handler.Send(result.Content.First().Buffer, result.Content.First().Offset, result.Content.First().Length, 0);
                var newState = new StateObject { WorkSocket = stateObject.WorkSocket, Session = stateObject.Session };
                newState.WorkSocket.BeginReceive(newState.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), newState);
                return;
            }

            if (result.Status == RequestResultStatus.EXIT_CONNECTION)
            {
                if (result.Content != null && result.Content.Any()) handler.Send(result.Content.First().Buffer, result.Content.First().Offset, result.Content.First().Length, 0);
                return;
            }

            if (result.Content == null || !result.Content.Any())
            {
                var newState = new StateObject { WorkSocket = stateObject.WorkSocket, Session = stateObject.Session };
                newState.WorkSocket.BeginReceive(newState.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), newState);
            }
            else
            {
                for (var i = 0; i < result.Content.Count() - 1; i++) handler.Send(result.Content.ElementAt(i).Buffer, result.Content.ElementAt(i).Offset, result.Content.ElementAt(i).Length, 0);
                handler.BeginSend(result.Content.Last().Buffer, result.Content.Last().Offset, result.Content.Last().Length, 0, new AsyncCallback(SendCallback), stateObject);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var newState = new StateObject { WorkSocket = state.WorkSocket, Session = state.Session };
            newState.WorkSocket.BeginReceive(newState.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), newState);
        }
    }
}
