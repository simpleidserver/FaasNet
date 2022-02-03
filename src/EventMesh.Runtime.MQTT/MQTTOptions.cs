using MQTTnet.Client.Options;

namespace EventMesh.Runtime.MQTT
{
    public class MQTTOptions
    {
        public MQTTOptions()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .WithCredentials("guest", "guest");
            MqttClientOptions = options.Build();
        }

        public IMqttClientOptions MqttClientOptions { get; set; }
    }
}
