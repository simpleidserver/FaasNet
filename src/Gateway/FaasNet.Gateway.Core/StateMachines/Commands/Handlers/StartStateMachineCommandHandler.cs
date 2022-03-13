using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Extensions;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Gateway.Core.StateMachines.Results;
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

namespace FaasNet.Gateway.Core.StateMachines.Commands.Handlers
{
    public class StartStateMachineCommandHandler : IRequestHandler<StartStateMachineCommand, StartStateMachineResult>
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IRuntimeEngine _runtimeEngine;

        public StartStateMachineCommandHandler(IWorkflowDefinitionRepository workflowDefinitionRepository, IRuntimeEngine runtimeEngine)
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
                throw new StateMachineNotFoundException(ErrorCodes.InvalidStateMachineName, string.Format(Global.UnknownStateMachine, request.Id));
            }

            if (!string.IsNullOrWhiteSpace(request.Input))
            {
                if (!request.Input.IsJson())
                {
                    throw new BadRequestException(ErrorCodes.InvalidExecutionInput, Global.InvalidExecutionInput);
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
            public ValidationResult(WorkflowDefinitionAggregate workflowDefinition, string data, Dictionary<string, string> parameters)
            {
                WorkflowDefinition = workflowDefinition;
                Data = data;
                Parameters = parameters;
            }

            public WorkflowDefinitionAggregate WorkflowDefinition { get; }
            public string Data { get; }
            public Dictionary<string, string> Parameters { get; set; }
        }

        #endregion
    }
}
