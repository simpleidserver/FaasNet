using FaasNet.Gateway.Core.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories
{
    public interface IFunctionCommandRepository
    {
        IQueryable<FunctionAggregate> Query();
        Task Add(FunctionAggregate function, CancellationToken cancellationToken);
        Task Delete(FunctionAggregate function, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
