using FaasNet.EventMesh.Core.Vpn.Queries;
using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn
{
    public class VpnService : IVpnService
    {
        private readonly IMediator _mediator;

        public VpnService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<VpnResult> Get(string name, CancellationToken cancellationToken)
        {
            return _mediator.Send(new GetVpnQuery { Vpn = name }, cancellationToken);
        }
    }
}
