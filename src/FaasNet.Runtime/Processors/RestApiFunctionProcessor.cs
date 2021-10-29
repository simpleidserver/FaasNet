﻿using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.OpenAPI;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public class RestApiFunctionProcessor : IFunctionProcessor
    {
        private readonly IOpenAPIParser _openAPIParser;

        public RestApiFunctionProcessor(IOpenAPIParser openAPIParser)
        {
            _openAPIParser = openAPIParser;
        }

        public WorkflowDefinitionTypes Type => WorkflowDefinitionTypes.REST;

        public async Task<JObject> Process(JObject input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState, CancellationToken cancellationToken)
        {
            var splitted = function.Operation.Split('#');
            if (splitted.Count() != 2)
            {
                throw new ProcessorException(string.Format(Global.BadRESTApiUrl, function.Operation));
            }

            return await _openAPIParser.Invoke(splitted.First(), splitted.Last(), input, cancellationToken);
        }
    }
}
