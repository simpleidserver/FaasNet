using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.AsyncAPI
{
    public interface IAsyncAPIParser
    {
        Task Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken);
    }
}
