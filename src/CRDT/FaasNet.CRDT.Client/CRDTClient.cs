﻿using FaasNet.CRDT.Client.Exceptions;
using FaasNet.CRDT.Client.Messages;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Client
{
    public class CRDTClient : BasePeerClient
    {
        public CRDTClient(IClientTransport clientTransport) : base(clientTransport) { }

        public async Task IncrementGCounter(string entityId, long increment, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.IncrementGCounter(entityId, increment, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = CRDTPackage.Deserialize(readCtx, true);
            Assert(package, packageResult);
        }

        public async Task IncrementPNCounter(string entityId, long increment, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.IncrementPNCounter(entityId, increment, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = CRDTPackage.Deserialize(readCtx, true);
            Assert(package, packageResult);
        }

        public async Task DecrementPNCounter(string entityId, long increment, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.DecrementPNCounter(entityId, increment, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = CRDTPackage.Deserialize(readCtx, true);
            Assert(package, packageResult);
        }

        public async Task AddGSet(string entityId, List<string> values, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.AddGSet(entityId, values, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = CRDTPackage.Deserialize(readCtx, true);
            Assert(package, packageResult);
        }

        public async Task<string> Get(string entityId, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.Get(entityId, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = CRDTPackage.Deserialize(readCtx, true);
            Assert(package, packageResult);
            return (packageResult as CRDTGetResultPackage).Value;
        }

        public async Task<CRDTSyncResultPackage> Sync(string peerId, string entityId, ICollection<ClockValue> clockVector, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = CRDTPackageRequestBuilder.Sync(peerId, entityId, clockVector, nonce);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await Send(payload, timeoutMS, cancellationToken);
            var resultPayload = await Receive(timeoutMS, cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload);
            var packageResult = CRDTPackage.Deserialize(readCtx, true);
            Assert(package, packageResult);
            return packageResult as CRDTSyncResultPackage;
        }

        private void Assert(CRDTPackage request, CRDTPackage result)
        {
            var error = result as CRDTErrorPackage;
            if (error != null) throw new CRDTClientException("Invalid request exception", error.Code);
            if (request.Nonce != result.Nonce) throw new CRDTClientException("Nonce doesn't match");
        }
    }
}