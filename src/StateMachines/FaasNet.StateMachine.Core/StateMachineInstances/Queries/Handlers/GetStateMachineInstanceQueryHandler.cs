using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Core.StateMachineInstances.Results;
using FaasNet.StateMachineInstance.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Queries.Handlers
{
    public class GetStateMachineInstanceQueryHandler : IRequestHandler<GetStateMachineInstanceQuery, StateMachineInstanceDetailsResult>
    {
        private readonly IStateMachineInstanceRepository _workflowInstanceRepository;

        public GetStateMachineInstanceQueryHandler(IStateMachineInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public async Task<StateMachineInstanceDetailsResult> Handle(GetStateMachineInstanceQuery request, CancellationToken cancellationToken)
        {
            var instance = await _workflowInstanceRepository.Get(request.Id, cancellationToken);
            if (instance == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_INSTANCE, string.Format(Global.UnknownStateMachineInstance, request.Id));
            }

            return StateMachineInstanceDetailsResult.ToDto(instance);
        }
    }
}
