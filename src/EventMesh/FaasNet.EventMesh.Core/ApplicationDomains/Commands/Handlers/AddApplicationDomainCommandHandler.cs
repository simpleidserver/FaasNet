﻿using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.ApplicationDomains.Commands.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Commands.Handlers
{
    public class AddApplicationDomainCommandHandler : IRequestHandler<AddApplicationDomainCommand, AddApplicationDomainResult>
    {
        private readonly IVpnService _vpnService;
        private readonly IApplicationDomainRepository _applicationDomainRepository;
        private readonly IBusControl _busControl;

        public AddApplicationDomainCommandHandler(IVpnService vpnService, IApplicationDomainRepository applicationDomainRepository, IBusControl busControl)
        {
            _vpnService = vpnService;
            _applicationDomainRepository = applicationDomainRepository;
            _busControl = busControl;
        }

        public async Task<AddApplicationDomainResult> Handle(AddApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnService.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            var applicationDomain = ApplicationDomain.Create(request.Vpn, request.Name, request.Description, request.RootTopic, request.StateMachineId);
            _applicationDomainRepository.Add(applicationDomain);
            await _applicationDomainRepository.SaveChanges(cancellationToken);
            return new AddApplicationDomainResult
            {
                Id = applicationDomain.Id
            };
        }
    }
}
