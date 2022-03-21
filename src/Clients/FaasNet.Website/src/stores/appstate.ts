import { createSelector } from '@ngrx/store';
import * as fromApplications from './application/reducers';
import * as fromEventMeshServers from './eventmeshservers/reducers';
import * as fromFunctions from './functions/reducers';
import * as fromServer from './server/reducers';
import * as fromStateMachineInstances from './statemachineinstances/reducers';
import * as fromStateMachines from './statemachines/reducers';
import * as fromVpns from './vpn/reducers';

export interface AppState {
  functions: fromFunctions.SearchFunctionsState;
  function: fromFunctions.FunctionState;
  stateMachines: fromStateMachines.SearchStateMachineState,
  stateMachine: fromStateMachines.StateMachineState,
  stateMachineInstances: fromStateMachineInstances.SearchStateMachineInstanceState,
  stateMachineInstance: fromStateMachineInstances.StateMachineInstanceState,
  eventMeshServers: fromEventMeshServers.EventMeshServersState,
  applicationDomains: fromApplications.ApplicationDomainsState,
  server: fromServer.ServerState,
  vpns: fromVpns.VpnLstState
  vpn: fromVpns.VpnState,
  clients: fromVpns.ClientLstState,
  client: fromVpns.ClientState,
  appDomains: fromVpns.ApplicationDomainLstState,
  appDomain: fromVpns.ApplicationDomainState,
  messageDefs : fromVpns.MessageDefinitionLstState
}

export const selectFunctions = (state: AppState) => state.functions;
export const selectFunction = (state: AppState) => state.function;
export const selectStateMachines = (state: AppState) => state.stateMachines;
export const selectStateMachine = (state: AppState) => state.stateMachine;
export const selectStateMachineInstances = (state: AppState) => state.stateMachineInstances;
export const selectStateMachineInstance = (state: AppState) => state.stateMachineInstance;
export const selectEventMeshServers = (state: AppState) => state.eventMeshServers;
export const selectApplicationDomains = (state: AppState) => state.applicationDomains;
export const selectServer = (state: AppState) => state.server;
export const selectVpns = (state: AppState) => state.vpns;
export const selectVpn = (state: AppState) => state.vpn;
export const selectClients = (state: AppState) => state.clients;
export const selectClient = (state: AppState) => state.client;
export const selectAppDomains = (state: AppState) => state.appDomains;
export const selectAppDomain = (state: AppState) => state.appDomain;
export const selectMessageDefs = (state: AppState) => state.messageDefs;

export const selectFunctionsResult = createSelector(
  selectFunctions,
  (state: fromFunctions.SearchFunctionsState) => {
    if (!state || state.Functions === null) {
      return null;
    }

    return state.Functions;
  }
);

export const selectFunctionConfigurationResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Configuration === null) {
      return null;
    }

    return state.Configuration;
  }
);

export const selectFunctionResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Function === null) {
      return null;
    }

    return state.Function;
  }
);

export const selectFunctionThreadsResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Threads === null) {
      return null;
    }

    return state.Threads;
  }
);

export const selectFunctionVirtualMemoryBytesResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.VirtualMemoryBytes === null) {
      return null;
    }

    return state.VirtualMemoryBytes;
  }
);

export const selectCpuUsageResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.CpuUsage === null) {
      return null;
    }

    return state.CpuUsage;
  }
);

export const selectRequestDurationResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.RequestDuration === null) {
      return null;
    }

    return state.RequestDuration;
  }
);

export const selectDetailsResult = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.Details === null) {
      return null;
    }

    return state.Details;
  }
);

export const selectTotalRequests = createSelector(
  selectFunction,
  (state: fromFunctions.FunctionState) => {
    if (!state || state.TotalRequests === null) {
      return null;
    }

    return state.TotalRequests;
  }
);

export const selectStateMachinesResult = createSelector(
  selectStateMachines,
  (state: fromStateMachines.SearchStateMachineState) => {
    if (!state || !state.StateMachines) {
      return null;
    }

    return state.StateMachines;
  }
);

export const selectStateMachineResult = createSelector(
  selectStateMachine,
  (state: fromStateMachines.StateMachineState) => {
    if (!state || !state.StateMachine) {
      return null;
    }

    return state.StateMachine;
  }
);

export const selectStateMachineInstancesResult = createSelector(
  selectStateMachineInstances,
  (state: fromStateMachineInstances.SearchStateMachineInstanceState) => {
    if (!state || !state.StateMachineInstances) {
      return null;
    }

    return state.StateMachineInstances;
  }
);

export const selectStateMachineInstanceResult = createSelector(
  selectStateMachineInstance,
  (state: fromStateMachineInstances.StateMachineInstanceState) => {
    if (!state || !state.StateMachineInstance) {
      return null;
    }

    return state.StateMachineInstance;
  }
);

export const selectEventMeshServersResult = createSelector(
  selectEventMeshServers,
  (state: fromEventMeshServers.EventMeshServersState) => {
    if (!state || !state.EventMeshServers) {
      return null;
    }

    return state.EventMeshServers;
  }
);

export const selectApplicationDomainsResult = createSelector(
  selectApplicationDomains,
  (state: fromApplications.ApplicationDomainsState) => {
    if (!state || !state.ApplicationDomains) {
      return null;
    }

    return state.ApplicationDomains;
  }
);

export const selectServerStatusResult = createSelector(
  selectServer,
  (state: fromServer.ServerState) => {
    if (!state || !state.Status) {
      return null;
    }

    return state.Status;
  }
);

export const selectVpnLstResult = createSelector(
  selectVpns,
  (state: fromVpns.VpnLstState) => {
    if (!state || !state.VpnLst) {
      return null;
    }

    return state.VpnLst;
  }
);

export const selectVpnResult = createSelector(
  selectVpn,
  (state: fromVpns.VpnState) => {
    if (!state || !state.Vpn) {
      return null;
    }

    return state.Vpn;
  }
);

export const selectClientsResult = createSelector(
  selectClients,
  (state: fromVpns.ClientLstState) => {
    if (!state || !state.ClientLst) {
      return null;
    }

    return state.ClientLst;
  }
);

export const selectClientResult = createSelector(
  selectClient,
  (state: fromVpns.ClientState) => {
    if (!state || !state.Client) {
      return null;
    }

    return state.Client;
  }
);

export const selectAppDomainsResult = createSelector(
  selectAppDomains,
  (state: fromVpns.ApplicationDomainLstState) => {
    if (!state || !state.ApplicationDomainLst) {
      return null;
    }

    return state.ApplicationDomainLst;
  }
);

export const selectAppDomainResult = createSelector(
  selectAppDomain,
  (state: fromVpns.ApplicationDomainState) => {
    if (!state || !state.ApplicationDomain) {
      return null;
    }

    return state.ApplicationDomain;
  }
);

export const selectMessageDefsResult = createSelector(
  selectMessageDefs,
  (state: fromVpns.MessageDefinitionLstState) => {
    if (!state || !state.MessageDefinitionLst) {
      return null;
    }

    return state.MessageDefinitionLst;
  }
);

export const appReducer = {
  functions: fromFunctions.getSearchFunctionsReducer,
  function: fromFunctions.getFunctionReducer,
  stateMachines: fromStateMachines.getSearchStateMachinesReducer,
  stateMachine: fromStateMachines.getStateMachineReducer,
  stateMachineInstances: fromStateMachineInstances.getSearchStateMachineInstancesReducer,
  stateMachineInstance: fromStateMachineInstances.getStateMachineInstanceReducer,
  eventMeshServers: fromEventMeshServers.getSearchEventMeshServersReducer,
  applicationDomains: fromApplications.getApplicationDomainsReducer,
  server: fromServer.getServerReducer,
  vpns: fromVpns.getVpnLstReducer,
  vpn: fromVpns.getVpnReducer,
  clients: fromVpns.getClientLstReducer,
  client: fromVpns.getClientReducer,
  appDomains: fromVpns.getApplicationDomainLstReducer,
  appDomain: fromVpns.getApplicationDomainReducer,
  messageDefs: fromVpns.getMessageDefLstReducer
};
