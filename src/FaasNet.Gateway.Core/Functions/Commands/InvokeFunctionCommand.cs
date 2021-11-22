using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.Functions.Commands
{
    public class InvokeFunctionCommand : IRequest<JToken>
    {
        public string Id { get; set; }
        public JObject Configuration { get; set; }
        public JToken Input { get; set; }
    }
}
