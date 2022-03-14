using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Core.AsyncApi.Queries
{
    public class GetAsyncApiOperationsQuery : IRequest<IEnumerable<Operation>>
    {
        public string Endpoint { get; set; }
    }
}
