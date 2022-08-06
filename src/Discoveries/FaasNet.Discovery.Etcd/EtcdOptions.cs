using FaasNet.Plugin;

namespace FaasNet.Discovery.Etcd
{
    public class EtcdOptions
    {
        public EtcdOptions()
        {
            ConnectionString = "http://localhost:23790";
            EventMeshPrefix = "eventmesh";
        }

        /// <summary>
        /// ETCD connectionstring.
        /// </summary>
        [PluginEntryOptionProperty("etcdConnectionString", "ETCD connectionstring")]
        public string ConnectionString { get; set; }
        /// <summary>
        /// ETCD username.
        /// </summary>
        [PluginEntryOptionProperty("etcdUsername", "ETCD username")]
        public string Username { get; set; }
        /// <summary>
        /// ETCD password.
        /// </summary>
        [PluginEntryOptionProperty("etcdPassword", "ETCD password")]
        public string Password { get; set; }
        /// <summary>
        /// URL prefix.
        /// </summary>
        [PluginEntryOptionProperty("etcdPrefix", "URL prefix")]
        public string EventMeshPrefix { get; set; }
    }
}
