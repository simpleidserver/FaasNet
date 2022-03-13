using FaasNet.Function.Core.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Repositories
{
    public class InMemoryFunctionRepository : IFunctionRepository
    {
        private readonly ICollection<FunctionAggregate> _functions;

        public InMemoryFunctionRepository(ICollection<FunctionAggregate> functions)
        {
            _functions = functions;
        }

        public Task Add(FunctionAggregate function, CancellationToken cancellationToken)
        {
            _functions.Add((FunctionAggregate)function.Clone());
            return Task.CompletedTask;
        }

        public Task Delete(FunctionAggregate function, CancellationToken cancellationToken)
        {
            _functions.Remove(function);
            return Task.CompletedTask;
        }

        public IQueryable<FunctionAggregate> Query()
        {
            return _functions.AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
