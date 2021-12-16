using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.Gateway.Core.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
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
            var function = FunctionAggregate.Create(command.Name, command.Image, command.Command);
            await _functionInvoker.Publish(function.Id, function.Image, function.Command, cancellationToken);
            await _functionInvoker.InitAudit(function.Id, cancellationToken);
            await _functionRepository.Add(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return function.Id;
        }
    }
}
