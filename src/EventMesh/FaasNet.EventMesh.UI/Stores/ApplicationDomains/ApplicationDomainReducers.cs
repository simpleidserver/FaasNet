using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ApplicationDomains
{
    public class ApplicationDomainReducers
    {
        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomain(ApplicationDomainState state, AddApplicationDomainAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomainFailureAction(ApplicationDomainState state, AddApplicationDomainResultAction action)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomainResult(ApplicationDomainState state, AddApplicationDomainResultAction action)
        {
            var records = state.ApplicationDomains.Records.ToList();
            records.Insert(0, new ApplicationDomainQueryResult { Name = action.Name, Description = action.Description, Vpn = action.Vpn, RootTopic = action.RootTopic });
            state.ApplicationDomains.Records = records;
            return state with
            {
                IsLoading = false,
                ApplicationDomains = state.ApplicationDomains
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceSearchApplicationDomains(ApplicationDomainState state, SearchApplicationDomainsAction action)
        {
            return state with
            {
                IsLoading =true,
                ApplicationDomains = new GenericSearchQueryResult<ApplicationDomainQueryResult>()
            };
        }


        [ReducerMethod]
        public static ApplicationDomainState ReduceSearchApplicationDomainsResult(ApplicationDomainState state, SearchApplicationDomainsResultAction action)
        {
            return state with
            {
                IsLoading = false,
                ApplicationDomains = action.ApplicationDomains
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceGetApplicationDomainAction(ApplicationDomainState state, GetApplicationDomainAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceGetApplicationDomainQueryResult(ApplicationDomainState state, GetApplicationDomainResultAction action)
        {
            return state with
            {
                IsLoading = false,
                ApplicationDomain = action.Content.Content
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomainElementAction(ApplicationDomainState state, AddApplicationDomainElementAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomainElementResultAction(ApplicationDomainState state, AddApplicationDomainElementResultAction action)
        {
            var elts = state.ApplicationDomain.Elements.ToList();
            elts.Add(new ApplicationDomainElementResult { CoordinateX = action.CoordinateX, CoordinateY = action.CoordinateY, ElementId = action.ElementId });
            return state with
            {
                IsLoading = false,
                ApplicationDomain = state.ApplicationDomain
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceRemoveApplicationDomainElementAction(ApplicationDomainState state, RemoveApplicationDomainElementAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceRemoveApplicationDomainElementResultAction(ApplicationDomainState state, RemoveApplicationDomainElementResultAction action)
        {
            var elts = state.ApplicationDomain.Elements.ToList();
            elts.Remove(elts.Single(e => e.ElementId == action.ElementId));
            return state with
            {
                IsLoading = false,
                ApplicationDomain = state.ApplicationDomain
            };
        }
    }
}
