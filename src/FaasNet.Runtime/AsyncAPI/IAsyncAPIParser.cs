using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.AsyncAPI
{
    public interface IAsyncAPIParser
    {
        bool TryParseUrl(string url, out AsyncApiResult operationResult);
        Task Invoke(string url, string operationId, JToken input, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}
