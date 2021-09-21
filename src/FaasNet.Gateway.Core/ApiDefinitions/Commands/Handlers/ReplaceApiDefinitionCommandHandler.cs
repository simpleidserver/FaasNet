using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Handlers
{
    public class ReplaceApiDefinitionCommandHandler : IRequestHandler<ReplaceApiDefinitionCommand, bool>
    {
        private readonly IApiDefinitionRepository _apiDefinitionRepository;

        public ReplaceApiDefinitionCommandHandler(IApiDefinitionRepository apiDefinitionRepository)
        {
            _apiDefinitionRepository = apiDefinitionRepository;
        }

        public async Task<bool> Handle(ReplaceApiDefinitionCommand request, CancellationToken cancellationToken)
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

        protected virtual void Apply(ApiDefinitionAggregate apiDef, ReplaceApiDefinitionCommand cmd)
        {
            apiDef.UpdatePath(cmd.Path);
            foreach(var op in cmd.Operations)
            {
                var operation = apiDef.UpdateOperation(op.Name, op.Path);
                foreach(var fn in op.Functions)
                {
                    operation.UpdateFunction(fn.Function, fn.SerializedConfiguration);
                    foreach(var fl in fn.Flows)
                    {
                        operation.UpdateSequenceFlow(fn.Function, fl.TargetRef);
                    }
                }
            }
        }
    }
}
