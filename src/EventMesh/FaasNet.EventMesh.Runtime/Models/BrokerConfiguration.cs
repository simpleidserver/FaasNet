using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class BrokerConfiguration
    {
        public BrokerConfiguration()
        {
            Records = new List<BrokerConfigurationRecord>();
        }

        public string Name { get; set; }
        public string Protocol { get; set; }
        public virtual ICollection<BrokerConfigurationRecord> Records { get; set; }

        public string GetValue(string name)
        {
            return Records.FirstOrDefault(s => s.Key == name)?.Value;
        }
    }
}
