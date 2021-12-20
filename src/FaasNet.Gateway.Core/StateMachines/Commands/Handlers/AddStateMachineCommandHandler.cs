using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Gateway.Core.StateMachines.Results;
using FaasNet.Runtime.Persistence;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.Gateway.Core.StateMachines.Commands.Handlers
{
    public class AddStateMachineCommandHandler : IRequestHandler<AddStateMachineCommand, AddStateMachineResult>
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IFunctionService _functionService;

        public AddStateMachineCommandHandler(
            IWorkflowDefinitionRepository workflowDefinitionRepository,
            IFunctionService functionService)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _functionService = functionService;
        }

        public async Task<AddStateMachineResult> Handle(AddStateMachineCommand request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _workflowDefinitionRepository.Query().FirstOrDefault(w=> w.Id == request.WorkflowDefinition.Id);
            if (workflowDefinition != null)
            {
                throw new BadRequestException(ErrorCodes.StateMachineExists, Global.StateMachineExists);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var customFunctions = request.WorkflowDefinition.Functions.Where(f => f.Type == Runtime.Domains.Enums.WorkflowDefinitionTypes.CUSTOM);
                foreach (var customFunction in customFunctions)
                {
                    var image = customFunction.Metadata.SelectToken("image").ToString();
                    var version = customFunction.Metadata.SelectToken("version").ToString();
                    var id = await _functionService.Publish(customFunction.Name, image, version, cancellationToken);
                    customFunction.FunctionId = id;
                }

                await _workflowDefinitionRepository.Add(request.WorkflowDefinition, cancellationToken);
                await _workflowDefinitionRepository.SaveChanges(cancellationToken);
                scope.Complete();
            }

            return new AddStateMachineResult
            {
                CreateDateTime = DateTime.UtcNow,
                Id = request.WorkflowDefinition.Id
            };
        }
    }
}
