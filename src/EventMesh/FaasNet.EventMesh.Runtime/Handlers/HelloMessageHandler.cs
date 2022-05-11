using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;
        private readonly IClientStore _clientStore;
        private readonly IClientSessionStore _clientSessionStore;
        private readonly RuntimeOptions _options;

        public HelloMessageHandler(IVpnStore vpnStore, IClientStore clientStore, IClientSessionStore clientSessionStore, IOptions<RuntimeOptions> options)
        {
            _vpnStore = vpnStore;
            _clientStore = clientStore;
            _clientSessionStore = clientSessionStore;
            _options = options.Value;
        }

        public Commands Command => Commands.HELLO_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var helloRequest = package as HelloRequest;
            await CheckVpn(helloRequest, cancellationToken);
            var client = await AuthenticateClient(helloRequest, cancellationToken);
            var sessionId = await AddSession(helloRequest, client, cancellationToken);
            var result = PackageResponseBuilder.Hello(package.Header.Seq, sessionId);
            return EventMeshPackageResult.SendResult(result);
        }

        private async Task CheckVpn(HelloRequest helloRequest, CancellationToken cancellationToken)
        {
            Vpn vpn = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get vpn"))
            {
                vpn = await _vpnStore.Get(helloRequest.UserAgent.Vpn, cancellationToken);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
            }

            if (vpn == null)
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.UNKNOWN_VPN);
            }
        }

        private async Task<Models.Client> AuthenticateClient(HelloRequest helloRequest, CancellationToken cancellationToken)
        {
            Models.Client client;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get client"))
            {
                client = await _clientStore.GetByClientId(helloRequest.UserAgent.Vpn, helloRequest.UserAgent.ClientId, cancellationToken);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
            }

            if (client == null)
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.INVALID_CLIENT);
            }

            if (!client.Purposes.Any(p => p == helloRequest.UserAgent.Purpose.Code))
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.NOT_AUTHORIZED);
            }

            return client;
        }

        private async Task<string> AddSession(HelloRequest helloRequest, Models.Client client, CancellationToken cancellationToken)
        {
            var session = ClientSession.Create(
                helloRequest.UserAgent.Environment,
                helloRequest.UserAgent.Pid,
                helloRequest.UserAgent.Purpose,
                helloRequest.UserAgent.BufferCloudEvents,
                client.ClientId,
                helloRequest.UserAgent.Vpn,
                helloRequest.UserAgent.IsServer ? ClientSessionTypes.SERVER : ClientSessionTypes.CLIENT,
                ComputeSessionExpirationDateTime(helloRequest),
                helloRequest.UserAgent.IsSessionInfinite);
            await _clientSessionStore.Add(session, cancellationToken);
            return session.Id;
        }

        private TimeSpan ComputeSessionExpirationDateTime(HelloRequest request)
        {
            if (request.UserAgent.Purpose == UserAgentPurpose.PUB)
            {
                if (request.UserAgent.IsSessionInfinite)
                {
                    throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.SESSION_LIFETIME_CANNOT_BE_INFINITE);
                }

                if (request.UserAgent.Expiration != null && request.UserAgent.Expiration.Value > _options.MaxPubSessionExpirationTimeSpan)
                {
                    throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.SESSION_LIFETIME_TOOLONG);
                }

                return request.UserAgent.Expiration ?? _options.DefaultPubSessionExpirationTimeSpan;
            }

            if (request.UserAgent.Expiration != null && request.UserAgent.Expiration.Value < _options.MinSubSessionExpirationTimeSpan)
            {
                throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.SESSION_LIFETIME_TOOSHORT);
            }

            return request.UserAgent.Expiration ?? _options.DefaultSubSessionExpirationTimeSpan;
        }
    }
}
