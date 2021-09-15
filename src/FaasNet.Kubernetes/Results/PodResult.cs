using System;

namespace FaasNet.Kubernetes.Results
{
    public class PodResult
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }

        public static PodResult Build(k8s.Models.V1Pod pod)
        {
            return new PodResult
            {
                Name = pod.Metadata.Name,
                Status = pod.Status.Phase,
                StartTime = pod.Status.StartTime
            };
        }
    }
}
