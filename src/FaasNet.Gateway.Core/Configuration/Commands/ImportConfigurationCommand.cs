using MediatR;

namespace FaasNet.Gateway.Core.Configuration.Commands
{
    public class ImportConfigurationCommand : IRequest<bool>
    {
        public string SerializedConfigurationFile { get; set; }
    }
}
