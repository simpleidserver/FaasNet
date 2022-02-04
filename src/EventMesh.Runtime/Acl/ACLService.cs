using EventMesh.Runtime.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Acl
{
    public class ACLService : IACLService
    {
        public Task<bool> Check(UserAgent userAgent, PossibleActions action, CancellationToken cancellationToken, string topic = null)
        {
            return Task.FromResult(true);
        }
    }
}
