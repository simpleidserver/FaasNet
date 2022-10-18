﻿using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.UI.Data;
using Fluxor;
using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.Stores.ApplicationDomains
{
    public class ApplicationDomainEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public ApplicationDomainEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(AddApplicationDomainAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.AddApplicationDomain(action.Name, action.Vpn, action.Description, action.RootTopic, action.Url, action.Port, CancellationToken.None);
            if (result.Status != AddApplicationDomainStatus.OK)
            {
                dispatcher.Dispatch(new AddApplicationDomainFailureAction($"An error occured while trying to add the application domain, Error: {Enum.GetName(typeof(AddApplicationDomainStatus), result.Status)}"));
                return;
            }

            dispatcher.Dispatch(new AddApplicationDomainResultAction { Description = action.Description, Name = action.Name, RootTopic = action.RootTopic, Vpn = action.Vpn });
        }

        [EffectMethod]
        public async Task Handle(SearchApplicationDomainsAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetAllApplicationDomains(action.Filter, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new SearchApplicationDomainsResultAction(result));
        }


        [EffectMethod]
        public async Task Handle(RemoveApplicationDomainLinkAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.RemoveLinkEventDefinition(action.Name, action.Vpn, action.Source, action.Target, action.EventId, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new RemoveApplicationDomainLinkResultAction { Name = action.Name, EventId = action.EventId, Vpn = action.Vpn, Result = result, Target = action.Target, Source = action.Source });
        }

        [EffectMethod]
        public async Task Handle(GetApplicationDomainAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetApplicationDomain(action.Name, action.Vpn, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new GetApplicationDomainResultAction { Content = result });
        }
    }

    public class AddApplicationDomainAction
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Vpn { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string RootTopic { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }

        public void Reset()
        {
            Name = String.Empty;
            Description = String.Empty;
            RootTopic = String.Empty;
        }
    }

    public class AddApplicationDomainFailureAction
    {
        public AddApplicationDomainFailureAction(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    public class AddApplicationDomainResultAction
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
    }

    public class SearchApplicationDomainsAction
    {
        public FilterQuery Filter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class SearchApplicationDomainsResultAction
    {
        public GenericSearchQueryResult<ApplicationDomainQueryResult> ApplicationDomains { get; }

        public SearchApplicationDomainsResultAction(GenericSearchQueryResult<ApplicationDomainQueryResult> applicationDomains)
        {
            ApplicationDomains = applicationDomains;
        }
    }

    public class GetApplicationDomainAction
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class GetApplicationDomainResultAction
    {
        public GetApplicationDomainResult Content { get; set; }
    }

    public class RemoveApplicationDomainLinkAction
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class RemoveApplicationDomainLinkResultAction
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }
        public RemoveLinkApplicationDomainResult Result { get; set; }
    }
}
