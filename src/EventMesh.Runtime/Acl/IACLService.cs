using EventMesh.Runtime.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Acl
{
    public interface IACLService
    {
        Task<bool> Check(UserAgent userAgent, PossibleActions action, CancellationToken cancellationToken, string topic = null);
    }
}
