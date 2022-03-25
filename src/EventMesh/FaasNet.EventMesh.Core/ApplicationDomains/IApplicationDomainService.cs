using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains
{
    public interface IApplicationDomainService
    {
        Task<ApplicationDomainResult> Get(string id, CancellationToken cancellationToken);
    }
}
