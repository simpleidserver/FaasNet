using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class GetAllVpnsMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;

        public GetAllVpnsMessageHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public Commands Command => Commands.GET_ALL_VPNS_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var vpns = await _vpnStore.GetAll(cancellationToken);
            return PackageResponseBuilder.GetAllVpns(package.Header.Seq, vpns.Select(v => v.Name));
        }
    }
}
