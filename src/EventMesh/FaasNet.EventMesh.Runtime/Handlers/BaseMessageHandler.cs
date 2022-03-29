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
        public BaseMessageHandler(IClientStore clientStore, IVpnStore vpnStore)
        {
            ClientStore = clientStore;
            VpnStore = vpnStore;
        }

        protected IClientStore ClientStore { get; private set; }
        protected IVpnStore VpnStore { get; private set; }

        protected async Task<ActiveSessionResult> GetActiveSession(Package requestPackage, string clientId, string sessionId, CancellationToken cancellationToken)
        {
            var client = await ClientStore.GetBySession(clientId, sessionId, cancellationToken);
            if (client == null)
            {
                throw new RuntimeException(requestPackage.Header.Command, requestPackage.Header.Seq, Errors.INVALID_SESSION);
            }

            var vpn = await VpnStore.Get(client.Vpn, cancellationToken);
            return new ActiveSessionResult(vpn, client);
        }

        protected class ActiveSessionResult
        {
            public ActiveSessionResult(Vpn vpn, Models.Client client)
            {
                Vpn = vpn;
                Client = client;
            }

            public Vpn Vpn { get; private set; }
            public Models.Client Client { get; private set; }
        }
    }
}
