using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionFunction
    {
        public string Name { get; set; }
        /// <summary>
        /// type = reqest :  <path_to_openapi_definition>#<operation_id>
        /// type = asyncapi : <path_to_asyncapi_definition>#<operation_id>
        /// type = rpc : <path_to_grpc_proto_file>#<service_name>#<service_method>
        /// type = graphsql :  <url_to_graphql_endpoint>#<literal "mutation" or "query">#<query_or_mutation_name>
        /// type = odata : <URI_to_odata_service>#<Entity_Set_Name>
        /// type = exprssion : workflow expression.
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// Defines the function type. Default is rest.
        /// </summary>
        [YamlIgnore]
        public WorkflowDefinitionTypes Type { get; set; }
        [YamlIgnore]
        public string MetadataStr { get; set; }
        /// <summary>
        /// Metadata information. Can be used to define custom function information.
        /// </summary>
        public JObject Metadata
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MetadataStr))
                {
                    return null;
                }

                return JObject.Parse(MetadataStr);
            }
        }
    }
}
