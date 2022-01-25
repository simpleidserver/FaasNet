namespace FaasNet.Runtime.AsyncAPI.v2.Models.Bindings.Amqp
{
    public class AmqpChannelBinding
    { 
        /// <summary>
        /// Defines whe type of channel is it.
        /// Can be either queue or routingKey.
        /// </summary>
        public AmqpChannelBindingIs Is { get; set; }
        /// <summary>
        /// When is=routingKey, this object defines the exchange properties.
        /// </summary>
        public AmqpChannelBindingExchange Exchange { get; set; }
        /// <summary>
        /// When is=queue, this object defines the queue properties.
        /// </summary>
        public AmqpChannelBindingQueue Queue { get; set; }
    }
}
