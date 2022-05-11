using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class BaseMessageHandler
    {
        public BaseMessageHandler(IClientSessionStore clientSessionStore, IVpnStore vpnStore)
        {
            ClientSessionStore = clientSessionStore;
            VpnStore = vpnStore;
        }

        protected IClientSessionStore ClientSessionStore { get; private set; }
        protected IVpnStore VpnStore { get; private set; }

        protected async Task<ActiveSessionResult> GetActiveSession(Package requestPackage, string sessionId, CancellationToken cancellationToken)
        {
            var clientSession = await ClientSessionStore.Get(sessionId, cancellationToken);
            if (clientSession == null)
            {
                throw new RuntimeException(requestPackage.Header.Command, requestPackage.Header.Seq, Errors.INVALID_SESSION);
            }

            var vpn = await VpnStore.Get(clientSession.Vpn, cancellationToken);
            return new ActiveSessionResult(vpn, clientSession);
        }

        protected class ActiveSessionResult
        {
            public ActiveSessionResult(Vpn vpn, ClientSession clientSession)
            {
                Vpn = vpn;
                ClientSession = clientSession;
            }

            public Vpn Vpn { get; private set; }
            public ClientSession ClientSession { get; private set; }
        }
    }
}
