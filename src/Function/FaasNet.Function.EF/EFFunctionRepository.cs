using FaasNet.Function.Core.Domains;
using FaasNet.Function.Core.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.EF
{
    public class EFFunctionRepository : IFunctionRepository
    {
        private readonly FunctionDBContext _dbContext;

        public EFFunctionRepository(FunctionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<FunctionAggregate> Query()
        {
            return _dbContext.Functions;
        }

        public Task Add(FunctionAggregate function, CancellationToken cancellationToken)
        {
            _dbContext.Functions.Add(function);
            return Task.CompletedTask;
        }

        public Task Delete(FunctionAggregate server, CancellationToken cancellationToken)
        {
            _dbContext.Functions.Remove(server);
            return Task.CompletedTask;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
