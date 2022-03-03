namespace FaasNet.EventMesh.Runtime
{
    public interface IRuntimeHost
    {
        void Run();
        void Stop();
    }
}
