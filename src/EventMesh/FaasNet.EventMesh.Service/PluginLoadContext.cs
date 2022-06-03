using System.Runtime.Loader;

namespace FaasNet.EventMesh.Service
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;


    }
}
