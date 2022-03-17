using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class AddClientCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public ICollection<int> Purposes { get; set; }
    }
}
