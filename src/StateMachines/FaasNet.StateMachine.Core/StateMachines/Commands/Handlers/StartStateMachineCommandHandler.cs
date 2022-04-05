using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Extensions;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Core.StateMachines.Results;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
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
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;
        // private readonly IStateMachineDefLauncher _stateMachineDefLauncher;

        public StartStateMachineCommandHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository /*, IStateMachineDefLauncher stateMachineDefLauncher*/)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
            // _stateMachineDefLauncher = stateMachineDefLauncher;
        }

        public async Task<StartStateMachineResult> Handle(StartStateMachineCommand request, CancellationToken cancellationToken)
        {
            var validationResult = Validate(request);
            // Quand un workflow instance est lancé alors il faut souscrire aux différents événements.
            // var workflowInstance = await _stateMachineDefLauncher.InstanciateAndLaunch(validationResult.WorkflowDefinition, request.Input, validationResult.Parameters, cancellationToken);
            /*
            return new StartStateMachineResult
            {
                LaunchDateTime = DateTime.UtcNow,
                Id = workflowInstance.Id  
            };
            */
            return null;
        }

        #region Validation

        protected virtual ValidationResult Validate(StartStateMachineCommand request)
        {
            var parameters = new Dictionary<string, string>();
            var workflowDef = _stateMachineDefinitionRepository.Query().FirstOrDefault(w => w.TechnicalId == request.Id);
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
