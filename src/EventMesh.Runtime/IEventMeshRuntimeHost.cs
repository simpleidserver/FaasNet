namespace EventMesh.Runtime
{
    public interface IEventMeshRuntimeHost
    {
        void Run(string ipAddr = "127.0.0.1", int port = 4889);
        void Stop();
    }
}
