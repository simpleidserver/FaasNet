using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Discovery.Etcd
{
    public class EtcdClusterStore : IClusterStore
    {
        private const string TOKEN_NAME = "token";
        private readonly EtcdOptions _options;
        private readonly int MAX_NB_RETRY = 5;
        private readonly int RETRY_TIMER_MS = 500;
        private readonly IEnumerable<IProtocolHandler> _protocols;

        public EtcdClusterStore(IOptions<EtcdOptions> options, IEnumerable<IProtocolHandler> protocols)
        {
            _options = options.Value;
            _protocols = protocols;
        }

        public Task SelfRegister(ClusterPeer peer, CancellationToken cancellationToken)
        {
            var protocol = _protocols.SingleOrDefault(p => p.MagicCode == "GOSSIP");
            if (protocol != null) throw new InvalidOperationException("ETCD Cluster cannot be used when GOSSIP protocol is enabled");
            return Retry(async () =>
            {
                var connection = EtcdConnectionPool.Build(_options);
                Grpc.Core.Metadata headers = null;
                if (connection.AuthResult != null) headers = new Grpc.Core.Metadata()
                {
                    new Grpc.Core.Metadata.Entry(TOKEN_NAME, connection.AuthResult.Token)
                };
                await connection.Client.PutAsync($"{_options.EventMeshPrefix}/{peer.Url}", JsonSerializer.Serialize(peer, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }), headers, cancellationToken: cancellationToken);
            }, 0);
        }

        public Task<IEnumerable<ClusterPeer>> GetAllNodes(CancellationToken cancellationToken)
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
                var result = new List<ClusterPeer>();
                foreach (var kvs in dic)
                {
                    result.Add(JsonSerializer.Deserialize<ClusterPeer>(kvs.Value, new JsonSerializerOptions
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

        private async Task<IEnumerable<ClusterPeer>> Retry(Func<Task<IEnumerable<ClusterPeer>>> callback, int nbRetry)
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
