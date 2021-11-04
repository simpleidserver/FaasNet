﻿using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories.Parameters;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories
{
    public interface IFunctionCommandRepository
    {
        Task<FunctionAggregate> Get(string name, CancellationToken cancellationToken);
        Task<FunctionAggregate> GetByImage(string image, CancellationToken cancellationToken);
        Task Add(FunctionAggregate function, CancellationToken cancellationToken);
        Task Delete(FunctionAggregate function, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}