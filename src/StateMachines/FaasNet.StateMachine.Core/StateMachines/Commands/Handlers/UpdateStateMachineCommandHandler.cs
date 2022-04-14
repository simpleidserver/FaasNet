using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Clients;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Core.StateMachines.Results;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class UpdateStateMachineCommandHandler : IRequestHandler<UpdateStateMachineCommand, AddStateMachineResult>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;
        private readonly IFunctionService _functionService;

        public UpdateStateMachineCommandHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository, IFunctionService functionService)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
            _functionService = functionService;
        }

        public async Task<AddStateMachineResult> Handle(UpdateStateMachineCommand request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _stateMachineDefinitionRepository.Query().FirstOrDefault(w=> w.TechnicalId == request.Id);
            if (workflowDefinition == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_DEF, string.Format(Global.UnknownStateMachineDef, request.WorkflowDefinition.Id));
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var customFunctions = request.WorkflowDefinition.Functions.Where(f => f.Type == StateMachineDefinitionTypes.CUSTOM);
                foreach (var customFunction in customFunctions)
                {
                    var image = customFunction.Metadata.SelectToken("image").ToString();
                    var version = customFunction.Metadata.SelectToken("version").ToString();
                    try
                    {
                        var fn = await _functionService.Get(image, version, cancellationToken);
                        customFunction.FunctionId = fn.Id;
                    }
                    catch
                    {
                        var id = await _functionService.Publish(new FunctionResult { Name = customFunction.Name, Image = image, Version = version }, cancellationToken);
                        customFunction.FunctionId = id;
                    }
                }

                workflowDefinition.IsLast = false;
                request.WorkflowDefinition.Id = workflowDefinition.Id;
                request.WorkflowDefinition.Version = workflowDefinition.Version + 1;
                request.WorkflowDefinition.Vpn = workflowDefinition.Vpn;
                request.WorkflowDefinition.ApplicationDomainId = workflowDefinition.ApplicationDomainId;
                request.WorkflowDefinition.Status = workflowDefinition.Status;
                request.WorkflowDefinition.RootTopic = workflowDefinition.RootTopic;
                request.WorkflowDefinition.CreateDateTime = DateTime.UtcNow;
                request.WorkflowDefinition.UpdateDateTime = DateTime.UtcNow;
                request.WorkflowDefinition.IsLast = true;
                request.WorkflowDefinition.RefreshTechnicalId();
                await _stateMachineDefinitionRepository.Update(workflowDefinition, cancellationToken);
                await _stateMachineDefinitionRepository.Add(request.WorkflowDefinition, cancellationToken);
                await _stateMachineDefinitionRepository.SaveChanges(cancellationToken);
                scope.Complete();
                return new AddStateMachineResult { CreateDateTime = request.WorkflowDefinition.CreateDateTime, Id = request.WorkflowDefinition.TechnicalId };
            }
        }
    }
}
