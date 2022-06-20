using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        private readonly int MAX_NB_RETRY = 5;
        private readonly int RETRY_TIMER_MS = 500;
        private readonly ILogger<EtcdClusterStore> _logger;

        public EtcdClusterStore(IOptions<EtcdOptions> options, ILogger<EtcdClusterStore> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task SelfRegister(ClusterNode node, CancellationToken cancellationToken)
        {
            return Retry(async () =>
            {
                var connection = EtcdConnectionPool.Build(_options);
                Grpc.Core.Metadata headers = null;
                if (connection.AuthResult != null) headers = new Grpc.Core.Metadata()
                {
                    new Grpc.Core.Metadata.Entry(TOKEN_NAME, connection.AuthResult.Token)
                };
                await connection.Client.PutAsync($"{_options.EventMeshPrefix}/{node.Url}", JsonSerializer.Serialize(node), headers, cancellationToken: cancellationToken);
            }, 0);
        }

        public Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            return Retry(async () =>
            {
                var connection = EtcdConnectionPool.Build(_options);
                Grpc.Core.Metadata headers = null;
                if (connection.AuthResult != null) headers = new Grpc.Core.Metadata()
                {
                    new Grpc.Core.Metadata.Entry(TOKEN_NAME, connection.AuthResult.Token)
                };
                var dic = await connection.Client.GetRangeValAsync($"{_options.EventMeshPrefix}/", headers);
                var result = new List<ClusterNode>();
                foreach (var kvs in dic)
                {
                    result.Add(JsonSerializer.Deserialize<ClusterNode>(kvs.Value, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }));
                }

                return result;
            }, 0);
        }

        private async Task Retry(Func<Task> callback, int nbRetry)
        {
            try
            {
                await callback();
            }
            catch
            {
                if (nbRetry >= MAX_NB_RETRY) throw;
                nbRetry++;
                Thread.Sleep(RETRY_TIMER_MS);
                await Retry(callback, nbRetry);
            }
        }

        private async Task<IEnumerable<ClusterNode>> Retry(Func<Task<IEnumerable<ClusterNode>>> callback, int nbRetry)
        {
            try
            {
                return await callback();
            }
            catch
            {
                if (nbRetry >= MAX_NB_RETRY) throw;
                nbRetry++;
                Thread.Sleep(RETRY_TIMER_MS);
                return await Retry(callback, nbRetry);
            }
        }
    }
}
