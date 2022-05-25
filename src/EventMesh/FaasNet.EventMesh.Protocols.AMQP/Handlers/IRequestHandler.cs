using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public interface IRequestHandler
    {
        string RequestName { get; }
        Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken);
    }
}
