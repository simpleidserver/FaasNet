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
    public class UpdateStateMachineCommandHandler : IRequestHandler<UpdateStateMachineCommand, AddStateMachineResult>
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IFunctionService _functionService;

        public UpdateStateMachineCommandHandler(
            IWorkflowDefinitionRepository workflowDefinitionRepository,
            IFunctionService functionService)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _functionService = functionService;
        }

        public async Task<AddStateMachineResult> Handle(UpdateStateMachineCommand request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _workflowDefinitionRepository.Query().FirstOrDefault(w=> w.TechnicalId == request.Id);
            if (workflowDefinition == null)
            {
                throw new StateMachineNotFoundException(ErrorCodes.UnknownStateMachine, string.Format(Global.UnknownStateMachine, request.WorkflowDefinition.Id));
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var customFunctions = request.WorkflowDefinition.Functions.Where(f => f.Type == Runtime.Domains.Enums.WorkflowDefinitionTypes.CUSTOM);
                foreach (var customFunction in customFunctions)
                {
                    var image = customFunction.Metadata.SelectToken("image").ToString();
                    var version = customFunction.Metadata.SelectToken("version").ToString();
                    try
                    {
                        var id = await _functionService.Publish(customFunction.Name, image, version, cancellationToken);
                        customFunction.FunctionId = id;
                    }
                    catch (FunctionAlreadyPublishedException ex)
                    {
                        customFunction.FunctionId = ex.FunctionId;
                    }
                }

                workflowDefinition.IsLast = false;
                request.WorkflowDefinition.Id = workflowDefinition.Id;
                request.WorkflowDefinition.Version = workflowDefinition.Version + 1;
                request.WorkflowDefinition.CreateDateTime = DateTime.UtcNow;
                request.WorkflowDefinition.UpdateDateTime = DateTime.UtcNow;
                request.WorkflowDefinition.IsLast = true;
                request.WorkflowDefinition.RefreshTechnicalId();
                await _workflowDefinitionRepository.Update(workflowDefinition, cancellationToken);
                await _workflowDefinitionRepository.Add(request.WorkflowDefinition, cancellationToken);
                await _workflowDefinitionRepository.SaveChanges(cancellationToken);
                scope.Complete();
                return new AddStateMachineResult { CreateDateTime = request.WorkflowDefinition.CreateDateTime, Id = request.WorkflowDefinition.TechnicalId };
            }
        }
    }
}
