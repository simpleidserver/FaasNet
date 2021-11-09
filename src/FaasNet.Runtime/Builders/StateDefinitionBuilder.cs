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

        public OperationStateBuilder Operation()
        {
            return new OperationStateBuilder();
        }

        public EventStateBuilder Event()
        {
            return new EventStateBuilder();
        }

        public SwitchStateBuilder Switch()
        {
            return new SwitchStateBuilder();
        }
    }
}
