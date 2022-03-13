using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Extensions;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Core.StateMachines.Results;
using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Persistence;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class StartStateMachineCommandHandler : IRequestHandler<StartStateMachineCommand, StartStateMachineResult>
    {
        private readonly IStateMachineDefinitionRepository _workflowDefinitionRepository;
        private readonly IRuntimeEngine _runtimeEngine;

        public StartStateMachineCommandHandler(IStateMachineDefinitionRepository workflowDefinitionRepository, IRuntimeEngine runtimeEngine)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _runtimeEngine = runtimeEngine;
        }

        public async Task<StartStateMachineResult> Handle(StartStateMachineCommand request, CancellationToken cancellationToken)
        {
            var validationResult = Validate(request);
            var workflowInstance = await _runtimeEngine.InstanciateAndLaunch(validationResult.WorkflowDefinition, request.Input, validationResult.Parameters, cancellationToken);
            return new StartStateMachineResult
            {
                LaunchDateTime = DateTime.UtcNow,
                Id = workflowInstance.Id  
            };
        }

        #region Validation

        protected virtual ValidationResult Validate(StartStateMachineCommand request)
        {
            var parameters = new Dictionary<string, string>();
            var workflowDef = _workflowDefinitionRepository.Query().FirstOrDefault(w => w.TechnicalId == request.Id);
            if (workflowDef == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_DEF, string.Format(Global.UnknownStateMachineDef, request.Id));
            }

            if (!string.IsNullOrWhiteSpace(request.Input))
            {
                if (!request.Input.IsJson())
                {
                    throw new DomainException(ErrorCodes.INVALID_INPUT, Global.InvalidInput);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Parameters))
            {
                parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.Parameters);
            }

            return new ValidationResult(workflowDef, request.Input, parameters);
        }

        protected class ValidationResult
        {
            public ValidationResult(StateMachineDefinitionAggregate workflowDefinition, string data, Dictionary<string, string> parameters)
            {
                WorkflowDefinition = workflowDefinition;
                Data = data;
                Parameters = parameters;
            }

            public StateMachineDefinitionAggregate WorkflowDefinition { get; }
            public string Data { get; }
            public Dictionary<string, string> Parameters { get; set; }
        }

        #endregion
    }
}
