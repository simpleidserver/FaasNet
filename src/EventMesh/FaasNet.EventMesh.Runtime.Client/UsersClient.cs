using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Runtime.Client.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Client
{
    public class UsersClient : EventMeshClient
    {
        public UsersClient(string clientId, string password, string vpn = Constants.DefaultVpn, string url = Constants.DefaultUrl, int port = Constants.DefaultPort, int bufferCloudEvents = 1) : base(clientId, password, vpn, url, port, bufferCloudEvents)
        {

        }

        public Task Publish(UserCreatedEvent userCreatedEvt, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Publish("userCreated", userCreatedEvt, cancellationToken);
        }

        public Task<SubscriptionResult> SubscribeUserCreated(Action<UserCreatedEvent> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SubscribeMessages("userCreated", callback, cancellationToken);
        }
    }
}
