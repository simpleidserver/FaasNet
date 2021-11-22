using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
{
    public class UnpublishFunctionCommandHandler : IRequestHandler<UnpublishFunctionCommand, bool>
    {
        private readonly IFunctionCommandRepository _functionRepository;
        private readonly IFunctionInvokerFactory _functionInvokerFactory;

        public UnpublishFunctionCommandHandler(
            IFunctionCommandRepository functionRepository,
            IFunctionInvokerFactory functionInvokerFactory)
        {
            _functionRepository = functionRepository;
            _functionInvokerFactory = functionInvokerFactory;
        }

        public async Task<bool> Handle(UnpublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == command.Id);
            if (function == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, Global.UnknownFunction);
            }

            var invoker = _functionInvokerFactory.Build(function.Provider);
            await invoker.Unpublish(command.Id, cancellationToken);
            await invoker.RemoveAudit(command.Id, cancellationToken);
            await _functionRepository.Delete(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
