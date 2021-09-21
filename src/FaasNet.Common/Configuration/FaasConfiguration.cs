using System.Collections.Generic;


namespace FaasNet.Common.Configuration
{
    public class FaasConfiguration
    {
        public FaasConfiguration()
        {
            Apis = new Dictionary<string, FaasApiConfiguration>();
            Functions = new List<FaasFunctionConfiguration>();
        }

        public FaasProviderConfiguration Provider { get; set; }
        public Dictionary<string, FaasApiConfiguration> Apis { get; set; }
        public ICollection<FaasFunctionConfiguration> Functions { get; set; }
    }
}
