using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Discovery.Etcd
{
    public class EtcdClusterStore : IClusterStore
    {
        private const string TOKEN_NAME = "token";
        private readonly EtcdOptions _options;

        public EtcdClusterStore(IOptions<EtcdOptions> options)
        {
            _options = options.Value;
        }

        public async Task SelfRegister(ClusterNode node, CancellationToken cancellationToken)
        {
            var connection = EtcdConnectionPool.Build(_options);
            Grpc.Core.Metadata headers = null;
            if (connection.AuthResult != null) headers = new Grpc.Core.Metadata()
            {
                new Grpc.Core.Metadata.Entry(TOKEN_NAME, connection.AuthResult.Token)
            };
            await connection.Client.PutAsync($"{_options.EventMeshPrefix}/{node.Port}", JsonSerializer.Serialize(node), headers, cancellationToken : cancellationToken);
        }

        public async Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            var connection = EtcdConnectionPool.Build(_options);
            Grpc.Core.Metadata headers = null;
            if (connection.AuthResult != null) headers = new Grpc.Core.Metadata()
            {
                new Grpc.Core.Metadata.Entry(TOKEN_NAME, connection.AuthResult.Token)
            };
            var dic = await connection.Client.GetRangeValAsync($"{_options.EventMeshPrefix}/", headers);
            var result = new List<ClusterNode>();
            foreach(var kvs in dic)
            {
                result.Add(JsonSerializer.Deserialize<ClusterNode>(kvs.Value, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }));
            }

            return result;
        }

        public Task<ClusterNode> GetNode(string url, int port, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
