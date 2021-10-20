using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Handlers
{
    public class AddApiDefinitionCommandHandler : IRequestHandler<AddApiDefinitionCommand, bool>
    {
        private readonly IApiDefinitionCommandRepository _apiDefinitionRepository;

        public AddApiDefinitionCommandHandler(IApiDefinitionCommandRepository apiDefinitionRepository)
        {
            _apiDefinitionRepository = apiDefinitionRepository;
        }

        public async Task<bool> Handle(AddApiDefinitionCommand request, CancellationToken cancellationToken)
        {
            var result = await _apiDefinitionRepository.GetByName(request.Name, cancellationToken);
            if (result != null)
            {
                throw new BusinessRuleException(string.Format(Global.FunctionExists, request.Name));
            }

            var apiDef = ApiDefinitionAggregate.Create(request.Name, request.Path);
            await _apiDefinitionRepository.AddOrUpdate(apiDef, cancellationToken);
            await _apiDefinitionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
