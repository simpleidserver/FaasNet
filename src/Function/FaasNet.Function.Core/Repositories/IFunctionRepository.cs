using FaasNet.Function.Core.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Repositories
{
    public interface IFunctionRepository
    {
        IQueryable<FunctionAggregate> Query();
        Task Add(FunctionAggregate function, CancellationToken cancellationToken);
        Task Delete(FunctionAggregate function, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
