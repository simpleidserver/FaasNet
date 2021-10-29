using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.OpenAPI
{
    public interface IOpenAPIParser
    {
        Task<JObject> Invoke(string url, string operationId, JObject input, CancellationToken cancellationToken);
    }
}
