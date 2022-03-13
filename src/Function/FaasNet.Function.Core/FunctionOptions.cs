namespace FaasNet.Function.Core
{
    public class FunctionOptions
    {
        public FunctionOptions()
        {
            FunctionApi = "http://faas-kubernetes-entry.faas.svc.cluster.local";
            PromotheusApi = "http://faas-prometheus-entry.faas.svc.cluster.local";
        }

        public string FunctionApi { get; set; }
        public string PrometheusFilePath { get; set; }
        public string PromotheusApi { get; set; }
    }
}
