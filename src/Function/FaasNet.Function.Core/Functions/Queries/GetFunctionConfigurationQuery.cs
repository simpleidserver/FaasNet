using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Function.Core.Functions.Queries
{
    public class GetFunctionConfigurationQuery : IRequest<JObject>
    {
        public string Id { get; set; }
    }
}
