using Amqp;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class OpenHandler : IRequestHandler
    {
        private readonly EventMeshAMQPOptions _options;

        public OpenHandler(IOptions<EventMeshAMQPOptions> options)
        {
            _options = options.Value;
        }

        public string RequestName => "amqp:open:list";

        public Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var openResult = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var cmdResult = new Amqp.Framing.Open
            {
                HostName = "localhost",
                MaxFrameSize = _options.MaxFrameSize,
                ChannelMax = _options.MaxChannel
            };
            return Task.FromResult(RequestResult.Ok(openResult.Serialize(cmdResult)));
        }
    }
}
