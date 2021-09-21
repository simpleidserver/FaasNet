using System.Collections.Generic;

namespace FaasNet.Common.Configuration
{
    public class FaasApiConfiguration
    {
        public string Path { get; set; }
        public ICollection<FaasApiOperationConfiguration> Operations { get; set; }
    }
}
