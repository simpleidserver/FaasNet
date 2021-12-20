using FaasNet.Gateway.Core.Domains;
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
            if (_functionRepository.Query().FirstOrDefault(f => f.Id == function.Id) != null)
            {
                throw new BadRequestException(ErrorCodes.FunctionExists, string.Format(Global.FunctionExists, function.Id));
            }

            await _functionInvoker.Publish(function.Id, function.Image, function.Version, function.Command, cancellationToken);
            await _functionInvoker.InitAudit(function.Id, cancellationToken);
            await _functionRepository.Add(function, cancellationToken);
            await _functionRepository.SaveChanges(cancellationToken);
            return function.Id;
        }
    }
}
