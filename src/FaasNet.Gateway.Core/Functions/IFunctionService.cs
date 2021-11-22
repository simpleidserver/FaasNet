using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions
{
    public interface IFunctionService
    {
        Task<string> Publish(string id, string provider, string metadata, CancellationToken cancellationToken);
        Task<JToken> Invoke(string id, JToken input, JObject configuration, CancellationToken cancellationToken);
        Task<bool> Unpublish(string id, CancellationToken cancellationToken);
    }
}
