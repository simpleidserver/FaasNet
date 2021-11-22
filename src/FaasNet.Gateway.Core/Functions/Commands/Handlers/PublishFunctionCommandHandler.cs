using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
{
    public class PublishFunctionCommandHandler : IRequestHandler<PublishFunctionCommand, string>
    {
        private readonly IFunctionCommandRepository _functionRepository;
        private readonly IFunctionInvokerFactory _functionInvokerFactory;

        public PublishFunctionCommandHandler(IFunctionCommandRepository functionRepository, IFunctionInvokerFactory functionInvokerFactory)
        {
            _functionRepository = functionRepository;
            _functionInvokerFactory = functionInvokerFactory;
        }

        public async Task<string> Handle(PublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var invoker = _functionInvokerFactory.Build(command.Provider);
            if (invoker == null)
            {
                throw new BadRequestException(ErrorCodes.UnsupportedFunctionProvider, Global.UnsupportedFunctionProvider);
            }

            var function = FunctionAggregate.Create(command.Name, command.Provider, command.Metadata);
            await invoker.Publish(function.Id, function.Metadata, cancellationToken);
            await invoker.InitAudit(function.Id, cancellationToken);
            await _functionRepository.Add(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return function.Id;
        }
    }
}
