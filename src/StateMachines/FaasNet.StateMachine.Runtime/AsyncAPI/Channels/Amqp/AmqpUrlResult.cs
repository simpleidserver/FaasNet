namespace FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp
{
    public class AmqpUrlResult
    {
        public AmqpUrlResult(string url, int? port)
        {
            Url = url;
            Port = port;
        }


        public string Url { get; }
        public int? Port { get; }
    }
}
