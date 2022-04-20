﻿using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public interface IMessageListener : IDisposable
    {
        string Name { get; }
        Task<IMessageListenerResult> Listen(Func<MessageResult, Task> callback, CancellationToken cancellationToken);
    }

    public interface IMessageListenerResult
    {
        Task Stop(CancellationToken cancellationToken);
    }

    public class MessageResult
    {
        public string Vpn { get; set; }
        public ICollection<CloudEvent> Content { get; set; }
        public string TopicMessage { get; set; }
    }
}
