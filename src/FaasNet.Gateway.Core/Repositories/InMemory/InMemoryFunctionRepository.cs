using FaasNet.Gateway.Core.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories.InMemory
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

        public Task<FunctionAggregate> Get(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_functions.FirstOrDefault(f => f.Name == name));
        }

        public Task<FunctionAggregate> GetByImage(string image, CancellationToken cancellationToken)
        {
            return Task.FromResult(_functions.FirstOrDefault(_ => _.Image == image));
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
