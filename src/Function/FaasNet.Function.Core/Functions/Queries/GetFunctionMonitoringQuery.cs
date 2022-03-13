using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Function.Core.Functions.Queries
{
    public class GetFunctionMonitoringQuery : IRequest<JObject>
    {
        public string Id { get; set; }
        public string Query { get; set; }
        public bool IsRange { get; set; }
    }
}
