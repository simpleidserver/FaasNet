using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.OpenAPI
{
    public interface IOpenAPIParser
    {
        bool TryParseUrl(string url, out OpenAPIUrlResult result);
        Task<JToken> Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken);
    }
}
