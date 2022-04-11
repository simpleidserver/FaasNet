using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Extensions;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Core.StateMachines.Results;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Serializer;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static FaasNet.StateMachine.WorkerHost.StateMachine;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class StartStateMachineCommandHandler : IRequestHandler<StartStateMachineCommand, StartStateMachineResult>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;
        private readonly StateMachineOptions _options;

        public StartStateMachineCommandHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository, IOptions<StateMachineOptions> options)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
            _options = options.Value;
        }

        public async Task<StartStateMachineResult> Handle(StartStateMachineCommand request, CancellationToken cancellationToken)
        {
            var serializer = new RuntimeSerializer();
            var validationResult = Validate(request);
            var channel = GrpcChannel.ForAddress(_options.StateMachineWorkerUrl);
            var client = new StateMachineClient(channel);
            var yaml = serializer.SerializeYaml(validationResult.WorkflowDefinition);
            var result = await client.LaunchAsync(new WorkerHost.StateMachineDef
            {
                Input = request.Input,
                Yaml = yaml,
                WorkflowDefTechnicalId = validationResult.WorkflowDefinition.TechnicalId
            });
            return new StartStateMachineResult
            {
                Id = result.Id,
                LaunchDateTime = result.LaunchDateTime.ToDateTime()
            };
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
