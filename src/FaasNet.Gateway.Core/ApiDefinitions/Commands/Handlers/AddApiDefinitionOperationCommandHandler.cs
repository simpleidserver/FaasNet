using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Handlers
{
    public class AddApiDefinitionOperationCommandHandler : IRequestHandler<AddApiDefinitionOperationCommand, bool>
    {
        private readonly IApiDefinitionCommandRepository _apiDefinitionRepository;

        public AddApiDefinitionOperationCommandHandler(IApiDefinitionCommandRepository apiDefinitionRepository)
        {
            _apiDefinitionRepository = apiDefinitionRepository;
        }

        public async Task<bool> Handle(AddApiDefinitionOperationCommand request, CancellationToken cancellationToken)
        {
            var result = await _apiDefinitionRepository.GetByName(request.ApiName, cancellationToken);
            if (result == null)
            {
                throw new ApiDefNotFoundException(string.Format(Global.UnknownApiDef, request.ApiName));
            }

            result.AddOperation(request.OpName, request.OpPath);
            await _apiDefinitionRepository.AddOrUpdate(result, cancellationToken);
            await _apiDefinitionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
