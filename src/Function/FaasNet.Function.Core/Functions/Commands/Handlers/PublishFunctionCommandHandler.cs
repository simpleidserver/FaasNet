using FaasNet.Domain.Exceptions;
using FaasNet.Function.Core.Domains;
using FaasNet.Function.Core.Functions.Invokers;
using FaasNet.Function.Core.Repositories;
using FaasNet.Function.Core.Resources;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Commands.Handlers
{
    public class PublishFunctionCommandHandler : IRequestHandler<PublishFunctionCommand, string>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IFunctionInvoker _functionInvoker;

        public PublishFunctionCommandHandler(IFunctionRepository functionRepository, IFunctionInvoker functionInvoker)
        {
            _functionRepository = functionRepository;
            _functionInvoker = functionInvoker;
        }

        public async Task<string> Handle(PublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var function = FunctionAggregate.Create(command.Name, command.Description, command.Image, command.Version, command.Command);
            var existingFunction = _functionRepository.Query().FirstOrDefault(f => f.Image == function.Image && f.Version == function.Version);
            if (existingFunction != null)
            {
                throw new DomainException(ErrorCodes.FUNCTION_ALREADY_EXISTS, string.Format(Global.FunctionExists, $"{function.Image}:{function.Version}"));
            }

            await _functionInvoker.Publish(function.Id, function.Image, function.Version, function.Command, cancellationToken);
            await _functionInvoker.InitAudit(function.Id, cancellationToken);
            await _functionRepository.Add(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return function.Id;
        }
    }
}
