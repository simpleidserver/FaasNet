using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ApplicationDomains
{
    public class ApplicationDomainReducers
    {
        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomain(ApplicationDomainState state, AddApplicationDomainAction action)
        {
            state.IsLoading = true;
            return state;
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomainFailureAction(ApplicationDomainState state, AddApplicationDomainResultAction action)
        {
            state.IsLoading = false;
            return state;
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceAddApplicationDomainResult(ApplicationDomainState state, AddApplicationDomainResultAction action)
        {
            state.IsLoading = false;
            var records = state.ApplicationDomains.Records.ToList();
            records.Insert(0, new ApplicationDomainQueryResult { Name = action.Name, Description = action.Description, Vpn= action.Vpn, RootTopic = action.RootTopic });
            state.ApplicationDomains.Records = records;
            return state;
        }

        [ReducerMethod]
        public static ApplicationDomainState ReduceSearchApplicationDomains(ApplicationDomainState state, SearchApplicationDomainsAction action) => new(isLoading: true, applicationDomains: null);


        [ReducerMethod]
        public static ApplicationDomainState ReduceSearchApplicationDomainsResult(ApplicationDomainState state, SearchApplicationDomainsResultAction action) => new(isLoading: false, applicationDomains: action.ApplicationDomains);
    }
}
