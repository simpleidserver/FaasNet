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

        public void Add(string id)
        {
            var targets = GetTargets();
            targets.Add(new PrometheusTarget
            {
                Labels = new PrometheusLabel
                {
                    Job = id
                },
                Targets = new List<string>
                {
                    $"{id}-entry.{id}.svc.cluster.local"
                }
            });
            Update(targets);
        }

        public void Remove(string id)
        {
            var targets = GetTargets();
            var filteredTargets = targets.Where(t => !(t.Labels.Job == id)).ToList();
            Update(filteredTargets);
        }

        private ICollection<PrometheusTarget> GetTargets()
        {
            if (string.IsNullOrWhiteSpace(_configuration.PrometheusFilePath) || !File.Exists(_configuration.PrometheusFilePath))
            {
                return new List<PrometheusTarget>();
            }

            var content = File.ReadAllText(_configuration.PrometheusFilePath);
            return JsonConvert.DeserializeObject<List<PrometheusTarget>>(content);
        }

        private void Update(ICollection<PrometheusTarget> targets)
        {
            if (string.IsNullOrWhiteSpace(_configuration.PrometheusFilePath))
            {
                return;
            }

            if (!File.Exists(_configuration.PrometheusFilePath))
            {
                File.Create(_configuration.PrometheusFilePath).Close();
            }

            File.WriteAllText(_configuration.PrometheusFilePath, JsonConvert.SerializeObject(targets));
        }
    }
}
