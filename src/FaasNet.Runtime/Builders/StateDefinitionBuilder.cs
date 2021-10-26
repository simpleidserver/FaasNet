namespace FaasNet.Runtime.Builders
{
    public class StateDefinitionBuilder
    {
        internal StateDefinitionBuilder()
        {

        }

        public InjectStateBuilder Inject()
        {
            return new InjectStateBuilder();
        }
    }
}
