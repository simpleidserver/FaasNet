using dotnet_etcd;
using Etcdserverpb;

namespace FaasNet.RaftConsensus.Discovery.Etcd
{
    public class EtcdConnectionPool
    {
        private static EtcdConnection _connection;

        public static EtcdConnection Build(EtcdOptions options)
        {
            if (_connection != null) return _connection;
            var client = new EtcdClient(options.ConnectionString);
            AuthenticateResponse authResult = null;
            if(!string.IsNullOrWhiteSpace(options.Username)&& !string.IsNullOrWhiteSpace(options.Password))
            {
                authResult = client.Authenticate(new AuthenticateRequest
                {
                    Name = options.Username,
                    Password = options.Password
                });
            }

            _connection = new EtcdConnection { Client = client, AuthResult = authResult };
            return _connection;
        }
    }

    public class EtcdConnection
    {
        public EtcdClient Client { get; set; }
        public AuthenticateResponse AuthResult { get; set; }
    }
}
