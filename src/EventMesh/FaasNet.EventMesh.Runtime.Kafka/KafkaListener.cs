using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Kafka
{
    public class KafkaListenerMessage
    {
        public long Offset { get; set; }
        public ConsumeResult<string?, byte[]> Message { get; set; }
    }

    public class KafkaListener
    {
        private readonly KafkaOptions _options;
        private readonly string _topic;
        private readonly string _clientId;
        private readonly string _sessionId;
        private readonly Action<string, string, string, ConsumeResult<string?, byte[]>> _callback;
        private CancellationTokenSource _cancellationTokenSource;
        private ICollection<KafkaListenerMessage> _messages;
        private IConsumer<string?, byte[]> _consumer;
        private static object _obj = new object();

        public KafkaListener(KafkaOptions options, string topic, string clientId, string sessionId, Action<string, string, string, ConsumeResult<string?, byte[]>> callback)
        {
            _options = options;
            _topic = topic;
            _clientId = clientId;
            _sessionId = sessionId;
            _callback = callback;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _messages = new List<KafkaListenerMessage>();
            var task = new Task(Handle, _cancellationTokenSource.Token);
            task.Start();
        }

        public void Consume(int nbEvts)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var kafkaMessage = _messages.ElementAt(nbEvts - 1);
            _consumer.Commit(kafkaMessage.Message);
            lock(_obj)
            {
                _messages = _messages.Where(m => m.Offset > kafkaMessage.Offset).ToList();
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messages = null;
        }

        public void Handle()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _clientId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            using (_consumer = new ConsumerBuilder<string, byte[]>(config).Build())
            {
                _consumer.Subscribe("^" + _topic);
                while(!_cancellationTokenSource.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                    lock (_obj)
                    {
                        _messages.Add(new KafkaListenerMessage { Message = consumeResult, Offset = consumeResult.Offset.Value });
                    }

                    _callback(_clientId, _sessionId, _topic, consumeResult);
                }
            }
        }
    }
}
