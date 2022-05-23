using Amqp;
using Amqp.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public interface IRequestHandler
    {
        string RequestName { get; }
        Task<IEnumerable<ByteBuffer>> Handle(StateObject state, DescribedList cmd, byte[] payload, ushort channel, CancellationToken cancellationToken);
    }
}
