using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Invokers
{
    public interface IFunctionInvoker
    {
        string Provider { get; }
        Task Publish(string id, JObject parameter, CancellationToken cancellationToken);
        Task InitAudit(string id, CancellationToken cancellationToken);
        Task<JToken> Invoke(string id, JToken input, JObject configuration, CancellationToken cancellationToken);
        Task Unpublish(string id, CancellationToken cancellationToken);
        Task RemoveAudit(string id, CancellationToken cancellationToken);
    }
}
