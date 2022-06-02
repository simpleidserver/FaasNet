namespace FaasNet.RaftConsensus.Discovery.Etcd
{
    public class EtcdOptions
    {
        public EtcdOptions()
        {
            ConnectionString = "https://localhost:23790";
            EventMeshPrefix = "eventmesh";
        }

        /// <summary>
        /// ETCD connectionstring.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// ETCD username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// ETCD password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// URL prefix.
        /// </summary>
        public string EventMeshPrefix { get; set; }
    }
}
