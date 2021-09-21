using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions
{
    public interface IFunctionService
    {
        Task<bool> Publish(string name, string image, CancellationToken cancellationToken);
        Task<bool> Unpublish(string name, CancellationToken cancellationToken);
    }
}
