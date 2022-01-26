using FaasNet.Runtime.AsyncAPI.v2.Models;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core.AsyncApi.Queries
{
    public class GetAsyncApiOperationsQuery : IRequest<IEnumerable<Operation>>
    {
        public string Endpoint { get; set; }
    }
}
