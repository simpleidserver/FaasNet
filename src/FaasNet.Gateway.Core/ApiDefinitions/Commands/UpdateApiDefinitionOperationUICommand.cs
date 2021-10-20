using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using MediatR;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands
{
    public class UpdateApiDefinitionOperationUICommand : IRequest<bool>
    {
        public string FuncName { get; set; }
        public string OperationName { get; set; }
        public ApiDefinitionOperationUIResult OperationUI { get; set; }
    }
}
