namespace FaasNet.Function.Core.Helpers
{
    public interface IPrometheusHelper
    {
        void Add(string name);
        void Remove(string name);
    }
}
