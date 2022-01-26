using FaasNet.Runtime.AsyncAPI.v2.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.AsyncAPI
{
    public interface IAsyncAPIParser
    {
        Task<AsyncApiDocument> GetConfiguration(string url, CancellationToken cancellationToken);
        bool TryParseUrl(string url, out AsyncApiResult operationResult);
        Task Invoke(string url, string operationId, JToken input, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}
