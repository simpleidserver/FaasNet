using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.Functions.Commands
{
    public class GetFunctionConfigurationCommand : IRequest<JObject>
    {
        public string FuncName { get; set; }
    }
}
