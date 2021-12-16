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
        private readonly IFunctionRepository _functionRepository;
        private readonly IFunctionInvoker _functionInvoker;

        public UnpublishFunctionCommandHandler(
            IFunctionRepository functionRepository,
            IFunctionInvoker functionInvoker)
        {
            _functionRepository = functionRepository;
            _functionInvoker = functionInvoker;
        }

        public async Task<bool> Handle(UnpublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == command.Id);
            if (function == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, Global.UnknownFunction);
            }

            await _functionInvoker.Unpublish(command.Id, cancellationToken);
            await _functionInvoker.RemoveAudit(command.Id, cancellationToken);
            await _functionRepository.Delete(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
