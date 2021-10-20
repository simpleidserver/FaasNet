using FaasNet.Gateway.Core.ApiDefinitions.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions
{
    public interface IApiDefinitionService
    {
        Task<bool> Replace(UpdateApiDefinitionCommand cmd, CancellationToken cancellationToken);
    }
}
