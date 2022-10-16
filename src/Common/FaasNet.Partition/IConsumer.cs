using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IConsumer<T> where T : class
    {
        Task Consume(T request, CancellationToken cancellationToken);
    }
}
