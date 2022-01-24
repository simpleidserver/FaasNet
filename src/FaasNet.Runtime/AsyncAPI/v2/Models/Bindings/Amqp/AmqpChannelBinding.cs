namespace FaasNet.Runtime.AsyncAPI.v2.Models.Bindings.Amqp
{
    public class AmqpChannelBinding
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the exchange.
        /// </summary>
        public string Type { get; set; }
    }
}
