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
        private readonly IFunctionCommandRepository _functionRepository;
        private readonly IFunctionInvokerFactory _functionInvokerFactory;

        public InvokeFunctionCommandHandler(
            IFunctionCommandRepository functionRepository,
            IFunctionInvokerFactory functionInvokerFactory)
        {
            _functionRepository = functionRepository;
            _functionInvokerFactory = functionInvokerFactory;
        }

        public Task<JToken> Handle(InvokeFunctionCommand request, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == request.Id);
            if (function == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, Global.UnknownFunction);
            }

            var invoker = _functionInvokerFactory.Build(function.Provider);
            if (invoker == null)
            {
                throw new BadRequestException(ErrorCodes.UnsupportedFunctionProvider, Global.UnsupportedFunctionProvider);
            }

            return invoker.Invoke(function.Id, request.Input, request.Configuration, cancellationToken);
        }
    }
}
