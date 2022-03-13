using FaasNet.Domain.Exceptions;
using FaasNet.Function.Core.Functions.Queries.Results;
using FaasNet.Function.Core.Repositories;
using FaasNet.Function.Core.Resources;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Queries.Handlers
{
    public class GetFunctionQueryHandler : IRequestHandler<GetFunctionQuery, FunctionResult>
    {
        private readonly IFunctionRepository _functionRepository;

        public GetFunctionQueryHandler(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public Task<FunctionResult> Handle(GetFunctionQuery request, CancellationToken cancellationToken)
        {
            var fn = _functionRepository.Query().FirstOrDefault(f => f.Id == request.Id);
            if (fn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_FUNCTION, string.Format(Global.UnknownFunction, request.Id));
            }

            return Task.FromResult(FunctionResult.ToDto(fn));
        }
    }
}
