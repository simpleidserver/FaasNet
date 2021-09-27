using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.Functions.Commands
{
    public class InvokeFunctionCommand : IRequest<JObject>
    {
        public string FuncName { get; set; }
        public JObject Configuration { get; set; }
        public JObject Input { get; set; }
    }
}
