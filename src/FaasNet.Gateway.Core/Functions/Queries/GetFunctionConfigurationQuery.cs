using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.Functions.Queries
{
    public class GetFunctionConfigurationQuery : IRequest<JObject>
    {
        public string Id { get; set; }
    }
}
