using System.Collections.Generic;


namespace FaasNet.Common.Configuration
{
    public class FaasApiFunctionConfiguration
    {
        public FaasApiFunctionConfiguration()
        {
            Flows = new List<FaasApiFlowConfiguration>();
        }

        public string Name { get; set; }
        public string Function { get; set; }
        public string Configuration { get; set; }
        public ICollection<FaasApiFlowConfiguration> Flows { get; set; }
    }
}
