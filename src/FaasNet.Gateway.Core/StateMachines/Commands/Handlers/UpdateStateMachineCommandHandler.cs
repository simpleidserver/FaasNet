using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Runtime.Persistence;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.Gateway.Core.StateMachines.Commands.Handlers
{
    public class UpdateStateMachineCommandHandler : IRequestHandler<UpdateStateMachineCommand, bool>
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

        public async Task<bool> Handle(UpdateStateMachineCommand request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _workflowDefinitionRepository.Query().FirstOrDefault(w=> w.Id == request.WorkflowDefinition.Id);
            if (workflowDefinition == null)
            {
                throw new StateMachineNotFoundException(ErrorCodes.UnknownStateMachine, string.Format(Global.UnknownStateMachine, request.WorkflowDefinition.Id));
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var customFunctions = request.WorkflowDefinition.Functions.Where(f => f.Type == Runtime.Domains.Enums.WorkflowDefinitionTypes.CUSTOM);
                foreach (var customFunction in customFunctions)
                {
                    var id = await _functionService.Publish(customFunction.Name, customFunction.Provider, customFunction.MetadataStr, cancellationToken);
                    customFunction.FunctionId = id;
                }

                request.WorkflowDefinition.States.First().IsRootState = true;
                workflowDefinition.Name = request.WorkflowDefinition.Name;
                workflowDefinition.Description = request.WorkflowDefinition.Description;
                workflowDefinition.UpdateDateTime = DateTime.UtcNow;
                workflowDefinition.Start = request.WorkflowDefinition.Start;
                workflowDefinition.UpdateStates(request.WorkflowDefinition.States);
                workflowDefinition.UpdateFunctions(request.WorkflowDefinition.Functions);
                workflowDefinition.UpdateEvents(request.WorkflowDefinition.Events);
                await _workflowDefinitionRepository.Update(workflowDefinition, cancellationToken);
                await _workflowDefinitionRepository.SaveChanges(cancellationToken);
                scope.Complete();
            }

            return true;
        }
    }
}
