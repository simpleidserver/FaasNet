using FaasNet.Gateway.Core.Domains;
using System.Collections.Generic;

namespace FaasNet.Gateway.Startup
{
    public class DefaultConfiguration
    {
        public static ICollection<ApiDefinitionAggregate> ApiDefinitions = new List<ApiDefinitionAggregate>
        {
            new ApiDefinitionAggregate
            {
                Name = "clients",
                Path = "clients",
                Operations = new List<ApiDefinitionOperation>
                {
                    new ApiDefinitionOperation
                    {
                        Path = string.Empty,
                        Functions = new List<ApiDefinitionFunction>
                        {
                            new ApiDefinitionFunction
                            {
                                Function = "getsql",
                                SerializedConfiguration = "{\r\n  \"ConnectionString\": \"Data Source=DESKTOP-F641MIJ\\\\SQLEXPRESS;Initial Catalog=OpenID;Integrated Security=True\",\r\n  \"SqlQuery\": \"SELECT * FROM [dbo].[Acrs]\"\r\n}"
                            },
                            new ApiDefinitionFunction
                            {
                                Function = "transform",
                                SerializedConfiguration = "{ \"mappings\": [ { \"input\": \"content.name\", \"output\": \"name\" } ] }"
                            }
                        },
                        SequenceFlows = new List<ApiDefinitionSequenceFlow>
                        {
                            new ApiDefinitionSequenceFlow
                            {
                                SourceRef = "getsql",
                                TargetRef = "transform"
                            }
                        }
                    }
                }
            }
        };
    }
}
