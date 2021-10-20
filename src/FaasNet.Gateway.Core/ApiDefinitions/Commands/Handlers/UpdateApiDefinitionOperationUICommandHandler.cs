using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Handlers
{
    public class UpdateApiDefinitionOperationUICommandHandler : IRequestHandler<UpdateApiDefinitionOperationUICommand, bool>
    {
        private readonly IApiDefinitionCommandRepository _apiDefinitionRepository;

        public UpdateApiDefinitionOperationUICommandHandler(IApiDefinitionCommandRepository apiDefinitionRepository)
        {
            _apiDefinitionRepository = apiDefinitionRepository;
        }

        public async Task<bool> Handle(UpdateApiDefinitionOperationUICommand request, CancellationToken cancellationToken)
        {
            var apiDefinition = await _apiDefinitionRepository.GetByName(request.FuncName, cancellationToken);
            if (apiDefinition == null)
            {
                throw new ApiDefNotFoundException(string.Format(Global.UnknownApiDef, request.FuncName));
            }

            apiDefinition.UpdateUIOperation(request.OperationName, request.OperationUI.ToDomain());
            await _apiDefinitionRepository.AddOrUpdate(apiDefinition, cancellationToken);
            await _apiDefinitionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
