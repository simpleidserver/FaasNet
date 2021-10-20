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
                        Name = "getAllClients",
                        Path = string.Empty,
                        Functions = new List<ApiDefinitionFunction>
                        {
                            new ApiDefinitionFunction
                            {
                                Function = "getsql",
                                SerializedConfiguration = "{\r\n  \"ConnectionString\": \"Data Source=mssql-entry.faas.svc.cluster.local;Initial Catalog=OpenID;User ID=sa;Password=D54DE7hHpkG9\",\r\n  \"SqlQuery\": \"SELECT * FROM [dbo].[Acrs]\"\r\n}"
                            },
                            new ApiDefinitionFunction
                            {
                                Function = "transform",
                                SerializedConfiguration = "{ \"mappings\": [ { \"input\": \"content[*].Name\", \"output\": \"name\" } ] }"
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
