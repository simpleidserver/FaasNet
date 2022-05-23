using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols
{
    public interface IProxy
    {
        Task Start();
        void Stop();
    }
}
