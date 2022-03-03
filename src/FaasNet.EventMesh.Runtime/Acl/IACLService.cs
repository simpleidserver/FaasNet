using FaasNet.EventMesh.Runtime.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Acl
{
    public interface IACLService
    {
        Task<bool> Check(UserAgent userAgent, PossibleActions action, CancellationToken cancellationToken, string topic = null);
    }
}
