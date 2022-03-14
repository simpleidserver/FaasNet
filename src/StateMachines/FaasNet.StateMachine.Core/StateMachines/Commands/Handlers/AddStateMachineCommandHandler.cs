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
    public class AddStateMachineCommandHandler : IRequestHandler<AddStateMachineCommand, AddStateMachineResult>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;
        private readonly IFunctionService _functionService;

        public AddStateMachineCommandHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository, IFunctionService functionService)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
            _functionService = functionService;
        }

        public async Task<AddStateMachineResult> Handle(AddStateMachineCommand request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _stateMachineDefinitionRepository.Query().FirstOrDefault(w=> w.Id == request.WorkflowDefinition.Id);
            if (workflowDefinition != null)
            {
                throw new DomainException(ErrorCodes.STATEMACHINE_ALREADY_EXISTS, Global.StateMachineExists);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var customFunctions = request.WorkflowDefinition.Functions.Where(f => f.Type == StateMachineDefinitionTypes.CUSTOM);
                foreach (var customFunction in customFunctions)
                {
                    var image = customFunction.Metadata.SelectToken("image").ToString();
                    var version = customFunction.Metadata.SelectToken("version").ToString();
                    var id = await _functionService.Publish(new FunctionResult { Name = customFunction.Name, Image = image, Version = version }, cancellationToken);
                    customFunction.FunctionId = id;
                }

                await _stateMachineDefinitionRepository.Add(request.WorkflowDefinition, cancellationToken);
                await _stateMachineDefinitionRepository.SaveChanges(cancellationToken);
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
