using FaasNet.Runtime.Domains.Definitions;
using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Builders
{
    public class KubernetesFunctionDefinitionBuilder : IFunctionBuilder
    {
        private string _name;
        private JObject _metadata;

        internal KubernetesFunctionDefinitionBuilder(string name)
        {
            _name = name;
            _metadata = new JObject();
            _metadata.Add("provider", "kubernetes");
        }

        public KubernetesFunctionDefinitionBuilder SetImage(string image)
        {
            _metadata.Add("image", image);
            return this;
        }

        public WorkflowDefinitionFunction Build()
        {
            return new WorkflowDefinitionFunction
            {
                Name = _name,
                Metadata = _metadata
            };
        }
    }
}
