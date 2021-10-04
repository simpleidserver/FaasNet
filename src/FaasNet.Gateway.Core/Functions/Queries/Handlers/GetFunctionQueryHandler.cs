using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Functions.Queries.Results;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
{
    public class GetFunctionQueryHandler : IRequestHandler<GetFunctionQuery, FunctionResult>
    {
        private readonly IFunctionQueryRepository _functionQueryRepository;

        public GetFunctionQueryHandler(IFunctionQueryRepository functionQueryRepository)
        {
            _functionQueryRepository = functionQueryRepository;
        }

        public async Task<FunctionResult> Handle(GetFunctionQuery request, CancellationToken cancellationToken)
        {
            var fn = await _functionQueryRepository.Get(request.FuncName, cancellationToken);
            if (fn == null)
            {
                throw new FunctionNotFoundException(string.Format(Global.UnknownFunction, request.FuncName));
            }

            return fn;
        }
    }
}
