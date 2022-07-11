using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public class MessageResult
    {
        public MessageResult(IEnumerable<byte> payload, Func<byte[], Task> responseCallback)
        {
            Payload = payload;
            ResponseCallback = responseCallback;
        }

        public IEnumerable<byte> Payload { get; private set; }
        public Func<byte[], Task> ResponseCallback { get; private set; }
    }
}
