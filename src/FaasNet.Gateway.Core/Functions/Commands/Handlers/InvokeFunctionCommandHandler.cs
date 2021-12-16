using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
{
    public class InvokeFunctionCommandHandler : IRequestHandler<InvokeFunctionCommand, JToken>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IFunctionInvoker _functionInvoker;

        public InvokeFunctionCommandHandler(
            IFunctionRepository functionRepository,
            IFunctionInvoker functionInvoker)
        {
            _functionRepository = functionRepository;
            _functionInvoker = functionInvoker;
        }

        public Task<JToken> Handle(InvokeFunctionCommand request, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == request.Id);
            if (function == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, Global.UnknownFunction);
            }

            return _functionInvoker.Invoke(function.Id, request.Input, request.Configuration, cancellationToken);
        }
    }
}
