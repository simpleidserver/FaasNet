using RabbitMQ.Client;
using System;

namespace EventMesh.Runtime.RabbitMQ
{
    public class RabbitMQOptions
    {
        public RabbitMQOptions()
        {
            ConnectionFactory = (c) =>
            {
                c.HostName = "localhost";
                c.Port = 5673;
                c.UserName = "guest";
                c.Password = "guest";
            };
        }

        public Action<ConnectionFactory> ConnectionFactory { get; set; }
    }
}
