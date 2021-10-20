using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Handlers
{
    public class UpdateApiDefinitionCommandHandler : IRequestHandler<UpdateApiDefinitionCommand, bool>
    {
        private readonly IApiDefinitionCommandRepository _apiDefinitionRepository;

        public UpdateApiDefinitionCommandHandler(IApiDefinitionCommandRepository apiDefinitionRepository)
        {
            _apiDefinitionRepository = apiDefinitionRepository;
        }

        public async Task<bool> Handle(UpdateApiDefinitionCommand request, CancellationToken cancellationToken)
        {
            var apiDefinition = await _apiDefinitionRepository.GetByName(request.Name, cancellationToken);
            if (apiDefinition == null)
            {
                apiDefinition = ApiDefinitionAggregate.Create(request.Name);
            }

            Apply(apiDefinition, request);
            await _apiDefinitionRepository.AddOrUpdate(apiDefinition, cancellationToken);
            await _apiDefinitionRepository.SaveChanges(cancellationToken);
            return true;
        }

        protected virtual void Apply(ApiDefinitionAggregate apiDef, UpdateApiDefinitionCommand cmd)
        {
            apiDef.UpdatePath(cmd.Path);
            apiDef.Operations.Clear();
            foreach(var op in cmd.Operations)
            {
                var operation = apiDef.AddOperation(op.Name, op.Path);
                foreach(var fn in op.Functions)
                {
                    operation.AddFunction(fn.Name, fn.Function, fn.SerializedConfiguration);
                    foreach(var fl in fn.Flows)
                    {
                        operation.AddSequenceFlow(fn.Function, fl.TargetRef);
                    }
                }
            }
        }
    }
}
