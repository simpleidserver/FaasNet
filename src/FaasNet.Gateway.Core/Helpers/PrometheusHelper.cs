using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FaasNet.Gateway.Core.Helpers
{
    public class PrometheusHelper : IPrometheusHelper
    {
        private readonly GatewayConfiguration _configuration;

        public PrometheusHelper(IOptions<GatewayConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public void Add(string name)
        {
            var targets = GetTargets();
            targets.Add(new PrometheusTarget
            {
                Labels = new PrometheusLabel
                {
                    Job = name
                },
                Targets = new List<string>
                {
                    $"{name}-entry.{name}.svc.cluster.local"
                }
            });
            Update(targets);
        }

        public void Remove(string name)
        {
            var targets = GetTargets();
            var filteredTargets = targets.Where(t => !(t.Labels.Job == name)).ToList();
            Update(filteredTargets);
        }

        private ICollection<PrometheusTarget> GetTargets()
        {
            var content = File.ReadAllText(_configuration.PrometheusFilePath);
            return JsonConvert.DeserializeObject<List<PrometheusTarget>>(content);
        }

        private void Update(ICollection<PrometheusTarget> targets)
        {
            File.WriteAllText(_configuration.PrometheusFilePath, JsonConvert.SerializeObject(targets));
        }
    }
}
