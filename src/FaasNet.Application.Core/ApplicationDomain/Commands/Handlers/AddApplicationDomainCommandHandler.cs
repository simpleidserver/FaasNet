using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.ApplicationDomain.Commands.Handlers
{
    public class AddApplicationDomainCommandHandler
    {
        public AddApplicationDomainCommandHandler()
        {

        }

        public async Task<bool> Handle(AddApplicationDomainCommand request, CancellationToken cancellationToken)
        {

            return false;
        }
    }
}
