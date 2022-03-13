using FaasNet.Domain.Exceptions;
using FaasNet.Function.Core.Functions.Invokers;
using FaasNet.Function.Core.Repositories;
using FaasNet.Function.Core.Resources;
using MediatR;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Commands.Handlers
{
    public class InvokeFunctionCommandHandler : IRequestHandler<InvokeFunctionCommand, JToken>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IFunctionInvoker _functionInvoker;

        public InvokeFunctionCommandHandler(IFunctionRepository functionRepository, IFunctionInvoker functionInvoker)
        {
            _functionRepository = functionRepository;
            _functionInvoker = functionInvoker;
        }

        public Task<JToken> Handle(InvokeFunctionCommand request, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == request.Id);
            if (function == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_FUNCTION, string.Format(Global.UnknownFunction, request.Id));
            }

            return _functionInvoker.Invoke(function.Id, request.Input, request.Configuration, cancellationToken);
        }
    }
}
