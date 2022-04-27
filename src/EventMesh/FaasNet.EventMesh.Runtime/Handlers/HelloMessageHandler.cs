using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;
        private readonly IClientStore _clientStore;
        private readonly RuntimeOptions _options;

        public HelloMessageHandler(IVpnStore vpnStore, IClientStore clientStore, IOptions<RuntimeOptions> options)
        {
            _vpnStore = vpnStore;
            _clientStore = clientStore;
            _options = options.Value;
        }

        public Commands Command => Commands.HELLO_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var helloRequest = package as HelloRequest;
            Models.Vpn vpn = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get vpn"))
            {
                vpn = await _vpnStore.Get(helloRequest.UserAgent.Vpn, cancellationToken);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
            }

            if (vpn == null)
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.UNKNOWN_VPN);
            }

            Models.Client client;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get client"))
            {
                client = await _clientStore.GetByClientId(vpn.Name, helloRequest.UserAgent.ClientId, cancellationToken);
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

            var sessionId = client.AddSession(sender,
                helloRequest.UserAgent.Environment,
                helloRequest.UserAgent.Pid,
                helloRequest.UserAgent.Purpose,
                helloRequest.UserAgent.BufferCloudEvents,
                helloRequest.UserAgent.IsServer,
                vpn.Name,
                ComputeExpiration(helloRequest),
                helloRequest.UserAgent.IsSessionInfinite);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return PackageResponseBuilder.Hello(package.Header.Seq, sessionId);
        }

        private TimeSpan ComputeExpiration(HelloRequest request)
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
