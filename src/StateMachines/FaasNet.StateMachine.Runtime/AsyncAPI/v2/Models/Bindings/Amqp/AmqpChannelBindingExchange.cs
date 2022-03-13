namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings.Amqp
{
    public class AmqpChannelBindingExchange
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the exchange.
        /// </summary>
        public AmqpChannelBindingExchangeType? Type { get; set; }
        /// <summary>
        /// Whether the exchange should survive broker restarts or not.
        /// </summary>
        public bool? Durable { get; set; }
        /// <summary>
        /// Whether the exchange should be deleted when the last queue is unbound from it.
        /// </summary>
        public bool? AutoDelete { get; set; }
        /// <summary>
        /// The virtual host of the exchange.
        /// </summary>
        public string VirtualHost { get; set; }
    }
}
