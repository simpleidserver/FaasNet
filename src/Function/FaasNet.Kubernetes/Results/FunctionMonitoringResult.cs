using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Kubernetes.Results
{
    public class FunctionMonitoringResult
    {
        public FunctionMonitoringResult()
        {
            Pods = new List<PodResult>();
        }

        public ICollection<PodResult> Pods { get; set; }

        public static FunctionMonitoringResult Build(k8s.Models.V1PodList lst)
        {
            return new FunctionMonitoringResult
            {
                Pods = lst.Items.Select(i => PodResult.Build(i)).ToList()
            };
        }
    }
}
