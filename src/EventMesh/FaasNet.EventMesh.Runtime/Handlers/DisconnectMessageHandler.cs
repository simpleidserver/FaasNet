using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class DisconnectMessageHandler : BaseMessageHandler, IMessageHandler
    {
        public DisconnectMessageHandler(IClientSessionStore clientSessionStore, IVpnStore vpnStore) : base(clientSessionStore, vpnStore)
        {
        }

        public Commands Command => Commands.DISCONNECT_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var disconnectRequest = package as DisconnectRequest;
            var sessionResult = await GetActiveSession(package, disconnectRequest.SessionId, cancellationToken);
            await CloseSession(sessionResult.ClientSession, cancellationToken);
            var result = PackageResponseBuilder.Disconnect(package.Header.Seq);
            return EventMeshPackageResult.SendResult(result);
        }

        private async Task CloseSession(ClientSession clientSession, CancellationToken cancellationToken)
        {
            clientSession.Close();
            await ClientSessionStore.Add(clientSession, cancellationToken);
        }
    }
}