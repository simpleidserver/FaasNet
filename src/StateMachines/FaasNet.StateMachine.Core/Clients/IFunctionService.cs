using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.Clients
{
    public interface IFunctionService
    {
        Task<FunctionResult> Get(string image, string version, CancellationToken cancellationToken);
        Task<string> Publish(FunctionResult functionResult, CancellationToken cancellationToken);
    }
}
