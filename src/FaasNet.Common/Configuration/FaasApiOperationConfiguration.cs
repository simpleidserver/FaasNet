using System.Collections.Generic;


namespace FaasNet.Common.Configuration
{
    public class FaasApiOperationConfiguration
    {
        public FaasApiOperationConfiguration()
        {
            Functions = new List<FaasApiFunctionConfiguration>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public ICollection<FaasApiFunctionConfiguration> Functions { get; set; }
    }
}
