using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public abstract class BaseSessionResult
    {
        public abstract Task<byte[]> ReceiveMessage();
        public abstract Task SendMessage(byte[] payload);
    }
}
