using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class BaseMessageHandler
    {
        public BaseMessageHandler(IVpnStore vpnStore)
        {
            VpnStore = vpnStore;
        }

        protected IVpnStore VpnStore { get; private set; }

        protected async Task<ActiveSessionResult> GetActiveSession(Package requestPackage, string clientId, string sessionId, CancellationToken cancellationToken)
        {
            var vpn = await VpnStore.Get(clientId, sessionId, cancellationToken);
            if (vpn == null)
            {
                throw new RuntimeException(requestPackage.Header.Command, requestPackage.Header.Seq, Errors.INVALID_SESSION);
            }

            var client = vpn.GetClient(clientId);
            return new ActiveSessionResult(vpn, client);
        }

        protected class ActiveSessionResult
        {
            public ActiveSessionResult(Vpn vpn, Client client)
            {
                Vpn = vpn;
                Client = client;
            }

            public Vpn Vpn { get; private set; }
            public Client Client { get; private set; }
        }
    }
}
