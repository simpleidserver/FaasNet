using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.Application.Core.ApplicationDomain.Queries
{
    public class GetAllApplicationDomainQuery : IRequest<IEnumerable<ApplicationDomainResult>>
    {
    }
}
