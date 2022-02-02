using System;

namespace EventMesh.Runtime
{
    public interface IMessageConsumer : IDisposable
    {
        void Start();
        void Stop();
        void Subscribe(string topic, string routingKey = null);
        void Unsubscribe(string topic, string routingKey = null);
    }
}
