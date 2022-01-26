using FaasNet.Runtime.AsyncAPI.v2.Models;

namespace FaasNet.Gateway.Core.OpenApi.Results
{
    public class GetAsyncApiOperationResult
    {
        public Operation AsyncApiOperation { get; set; }
        public Components Components { get; set; }
    }
}
