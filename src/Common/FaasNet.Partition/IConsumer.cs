using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IConsumer<T> : IDisposable where T : class
    {
        Task Consume(T request, CancellationToken cancellationToken);
    }
}
