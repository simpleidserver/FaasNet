using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Newtonsoft.Json.Linq;

namespace FaasNet.StateMachine.Runtime.Builders
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

        public StateMachineDefinitionFunction Build()
        {
            return new StateMachineDefinitionFunction
            {
                Name = _name,
                Metadata = _metadata
            };
        }
    }
}
