namespace FaasNet.Gateway.Core
{
    public class GatewayConfiguration
    {
        public GatewayConfiguration()
        {
            FunctionApi = $"http://faas-kubernetes-entry.faas.svc.cluster.local";
        }

        public string FunctionApi { get; set; }
        public string PrometheusFilePath { get; set; }
    }
}
