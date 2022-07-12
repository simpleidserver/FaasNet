using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Peer
{
    public interface IProtocolHandlerFactory
    {
        IProtocolHandler Build(string magicCode);
    }

    public class ProtocolHandlerFactory : IProtocolHandlerFactory
    {
        private readonly IEnumerable<IProtocolHandler> _protocolHandlers;

        public ProtocolHandlerFactory(IEnumerable<IProtocolHandler> protocolHandlers)
        {
            _protocolHandlers = protocolHandlers;
        }

        public IProtocolHandler Build(string magicCode)
        {
            return _protocolHandlers.FirstOrDefault(p => p.MagicCode == magicCode);
        }
    }
}
