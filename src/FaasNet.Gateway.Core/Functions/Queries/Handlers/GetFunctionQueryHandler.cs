using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions.Queries.Results;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
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
            var fn = _functionRepository.Query().FirstOrDefault(f => f.Name == request.FuncName);
            if (fn == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, string.Format(Global.UnknownFunction, request.FuncName));
            }

            return Task.FromResult(FunctionResult.ToDto(fn));
        }
    }
}
