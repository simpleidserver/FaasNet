using Amqp;
using Amqp.Sasl;
using Amqp.Types;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class SASLInitHandler : IRequestHandler
    {
        public string RequestName => "amqp:sasl-init:list";

        public Task<IEnumerable<ByteBuffer>> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var saslInit = parameter.Cmd as SaslInit;
            if (saslInit.Mechanism.ToString() != "PLAIN") throw new InvalidOperationException("Only PLAIN authentication mechanisme is supported");
            var saslInitResult = new Frame { Channel = 0, Type = FrameTypes.Sasl };
            var salsOutcome = new SaslOutcome { Code = SaslCode.Auth };
            if (TryExtractCredentials(saslInit, out StateSessionObject stateSession))
            {
                state.Session = stateSession;
                salsOutcome.Code = SaslCode.Ok;
            }

            IEnumerable<ByteBuffer> result = new[]
            {
                saslInitResult.Serialize(salsOutcome)
            };
            return Task.FromResult(result);
        }

        private bool TryExtractCredentials(SaslInit init, out StateSessionObject stateSession)
        {
            byte[] response = init.InitialResponse;
            if (response.Length > 0)
            {
                string message = Encoding.UTF8.GetString(response, 0, response.Length);
                string[] items = message.Split('\0');
                if (items.Length == 3)
                {
                    stateSession = new StateSessionObject
                    {
                        ClientId = items[1],
                        Password = items[2]
                    };
                    return true;
                }
            }

            stateSession = null;
            return false;
        }
    }
}
