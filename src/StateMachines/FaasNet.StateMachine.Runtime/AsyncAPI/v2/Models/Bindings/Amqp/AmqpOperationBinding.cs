using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings.Amqp
{
    public class AmqpOperationBinding
    {
        /// <summary>
        /// TTL (Time-To-Live) for the message. It MUST be greater than or equal to zero.
        /// </summary>
        public int Expiration { get; set; }
        /// <summary>
        /// Identifies the user who has sent the message.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// The routing keys the message should be routed to at the time of publishing.
        /// </summary>
        public IEnumerable<string> Cc { get; set; }
        /// <summary>
        /// A priority for the message.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Whether the message is mandatory or not.
        /// </summary>
        public bool Mandatory { get; set; }
    }
}
