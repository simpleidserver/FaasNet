using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.EF.Persistence
{
    public class FunctionRepository : IFunctionRepository
    {
        private readonly GatewayDBContext _dbContext;

        public FunctionRepository(GatewayDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(FunctionAggregate function, CancellationToken cancellationToken)
        {
            _dbContext.Functions.Add(function);
            return Task.CompletedTask;
        }

        public Task Delete(FunctionAggregate function, CancellationToken cancellationToken)
        {
            _dbContext.Functions.Remove(function);
            return Task.CompletedTask;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<FunctionAggregate> Query()
        {
            return _dbContext.Functions;
        }
    }
}
