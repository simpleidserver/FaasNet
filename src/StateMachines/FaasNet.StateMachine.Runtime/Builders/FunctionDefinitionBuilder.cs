namespace FaasNet.StateMachine.Runtime.Builders
{
    public class FunctionDefinitionBuilder
    {
        public RESTAPIFunctionDefinitionBuilder RestAPI(string name, string operation)
        {
            return new RESTAPIFunctionDefinitionBuilder(name, operation);
        }

        public ASYNCAPIFunctionDefinitionBuilder AsyncAPI(string name, string operation)
        {
            return new ASYNCAPIFunctionDefinitionBuilder(name, operation);
        }

        public KubernetesFunctionDefinitionBuilder KubernetesAPI(string name)
        {
            return new KubernetesFunctionDefinitionBuilder(name);
        }
    }
}
