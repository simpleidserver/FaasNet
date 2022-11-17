namespace FaasNet.EventMesh.Performance
{
    public interface IBenchmarkScenario
    {
        Task Setup(BenchmarkGlobalContext context);
        Task Execute(BenchmarkGlobalContext context);
        void Cleanup();
    }
}
