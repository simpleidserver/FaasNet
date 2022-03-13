using FaasNet.Domain.Exceptions;
using FaasNet.Function.Core.Functions.Invokers;
using FaasNet.Function.Core.Repositories;
using FaasNet.Function.Core.Resources;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Commands.Handlers
{
    public class UnpublishFunctionCommandHandler : IRequestHandler<UnpublishFunctionCommand, bool>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IFunctionInvoker _functionInvoker;

        public UnpublishFunctionCommandHandler(IFunctionRepository functionRepository, IFunctionInvoker functionInvoker)
        {
            _functionRepository = functionRepository;
            _functionInvoker = functionInvoker;
        }

        public async Task<bool> Handle(UnpublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == command.Id);
            if (function == null)
            {
                throw new DomainException(ErrorCodes.UNKNOWN_FUNCTION, string.Format(Global.UnknownFunction, command.Id));
            }

            await _functionInvoker.Unpublish(command.Id, cancellationToken);
            await _functionInvoker.RemoveAudit(command.Id, cancellationToken);
            await _functionRepository.Delete(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
