using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Invokers
{
    public interface IFunctionInvoker
    {
        Task Publish(string id, string image, string version, string command, CancellationToken cancellationToken);
        Task InitAudit(string id, CancellationToken cancellationToken);
        Task<JToken> Invoke(string id, JToken input, JObject configuration, CancellationToken cancellationToken);
        Task Unpublish(string id, CancellationToken cancellationToken);
        Task RemoveAudit(string id, CancellationToken cancellationToken);
    }
}
