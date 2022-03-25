using FaasNet.EventMesh.Core.Clients.Commands.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Clients.Commands
{
    public class AddClientCommand : IRequest<AddClientResult>
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public ICollection<int> Purposes { get; set; }
    }
}
