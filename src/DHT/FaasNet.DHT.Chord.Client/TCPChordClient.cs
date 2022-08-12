﻿using FaasNet.Common.Helpers;
using FaasNet.DHT.Chord.Client.Extensions;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.Peer.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace FaasNet.DHT.Chord.Client
{
    public class TCPChordClient : IDisposable
    {
        private const int BUFFER_SIZE = 1024;
        private readonly string _url;
        private readonly int _port;
        private readonly IPAddress _ipAddr;
        private Socket _socket;

        public TCPChordClient(string url = Constants.DefaultUrl, int port = Constants.DefaultPort)
        {
            _url = url;
            _port = port;
            _ipAddr = DnsHelper.ResolveIPV4(url);
            _socket = CreateSession();
        }

        public string Url => _url;
        public int Port => _port;

        public void Create(int dimFingerTable, int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.Create(dimFingerTable);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
        }

        public void Join(string url, int port, int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.Join(url, port);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
        }

        public int GetDimensionFingerTable(int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.GetDimensionFingerTable();
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as GetDimensionFingerTableResult;
            return result.Dimension;
        }

        public FindSuccessorResult FindSuccessor(long nodeId, int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.FindSuccessor(nodeId);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            if(nodeId == 9)
            {
                string sss = "";
            }

            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);

            if (nodeId == 9)
            {
                string sss = "";
            }

            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as FindSuccessorResult;
            return result;
        }

        public FindPredecessorResult FindPredecessor(int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.FindPredecessor();
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as FindPredecessorResult;
            return result;
        }

        public void Notify(string url, int port, long id, int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.Notify(url, port, id);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
        }

        public string GetKey(long id, int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.GetKey(id);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
            var readBufferContext = new ReadBufferContext(payload);
            var result = ChordPackage.Deserialize(readBufferContext) as GetKeyResult;
            return result.Value;
        }

        public void AddKey(long id, string value, bool force = false, int timeoutMS = 500)
        {
            var request = PackageRequestBuilder.AddKey(id, value, force);
            var writeBufferContext = new WriteBufferContext();
            request.SerializeEnvelope(writeBufferContext);
            _socket.Send(writeBufferContext.Buffer.ToArray(), 0, timeoutMS);
            var payload = new byte[BUFFER_SIZE];
            _socket.Receive(payload, 0, timeoutMS);
        }

        public void Dispose()
        {
            if(_socket != null)
            {
                _socket.Close();
            }
        }

        private Socket CreateSession()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var idp = new IPEndPoint(_ipAddr, _port);
            socket.Connect(idp);
            return socket;
        }
    }
}