namespace FaasNet.Runtime.Builders
{
    public class FunctionDefinitionBuilder
    {
        public RESTAPIFunctionDefinitionBuilder RestAPI(string name, string operation)
        {
            return new RESTAPIFunctionDefinitionBuilder(name, operation);
        }
    }
}
