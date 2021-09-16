using FaasNet.Gateway.Core.ApiDefinitions.Commands.Results;
using MediatR;
using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands
{
    public class InvokeApiDefinitionCommand : IRequest<InvokeApiDefinitionResult>
    {
        public string FullPath { get; set; }
        public JObject Content { get; set; }
    }
}
