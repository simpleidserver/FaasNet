import { createSelector } from '@ngrx/store';
import * as fromApplications from './application/reducers';
import * as fromEventMeshServers from './eventmeshservers/reducers';
import * as fromFunctions from './functions/reducers';
import * as fromServer from './server/reducers';
import * as fromStateMachineInstances from './statemachineinstances/reducers';
import * as fromStateMachines from './statemachines/reducers';
import * as fromVpns from './vpn/reducers';
export const selectFunctions = (state) => state.functions;
export const selectFunction = (state) => state.function;
export const selectStateMachines = (state) => state.stateMachines;
export const selectStateMachine = (state) => state.stateMachine;
export const selectStateMachineInstances = (state) => state.stateMachineInstances;
export const selectStateMachineInstance = (state) => state.stateMachineInstance;
export const selectEventMeshServers = (state) => state.eventMeshServers;
export const selectApplicationDomains = (state) => state.applicationDomains;
export const selectServer = (state) => state.server;
export const selectVpns = (state) => state.vpns;
export const selectFunctionsResult = createSelector(selectFunctions, (state) => {
    if (!state || state.Functions === null) {
        return null;
    }
    return state.Functions;
});
export const selectFunctionConfigurationResult = createSelector(selectFunction, (state) => {
    if (!state || state.Configuration === null) {
        return null;
    }
    return state.Configuration;
});
export const selectFunctionResult = createSelector(selectFunction, (state) => {
    if (!state || state.Function === null) {
        return null;
    }
    return state.Function;
});
export const selectFunctionThreadsResult = createSelector(selectFunction, (state) => {
    if (!state || state.Threads === null) {
        return null;
    }
    return state.Threads;
});
export const selectFunctionVirtualMemoryBytesResult = createSelector(selectFunction, (state) => {
    if (!state || state.VirtualMemoryBytes === null) {
        return null;
    }
    return state.VirtualMemoryBytes;
});
export const selectCpuUsageResult = createSelector(selectFunction, (state) => {
    if (!state || state.CpuUsage === null) {
        return null;
    }
    return state.CpuUsage;
});
export const selectRequestDurationResult = createSelector(selectFunction, (state) => {
    if (!state || state.RequestDuration === null) {
        return null;
    }
    return state.RequestDuration;
});
export const selectDetailsResult = createSelector(selectFunction, (state) => {
    if (!state || state.Details === null) {
        return null;
    }
    return state.Details;
});
export const selectTotalRequests = createSelector(selectFunction, (state) => {
    if (!state || state.TotalRequests === null) {
        return null;
    }
    return state.TotalRequests;
});
export const selectStateMachinesResult = createSelector(selectStateMachines, (state) => {
    if (!state || !state.StateMachines) {
        return null;
    }
    return state.StateMachines;
});
export const selectStateMachineResult = createSelector(selectStateMachine, (state) => {
    if (!state || !state.StateMachine) {
        return null;
    }
    return state.StateMachine;
});
export const selectStateMachineInstancesResult = createSelector(selectStateMachineInstances, (state) => {
    if (!state || !state.StateMachineInstances) {
        return null;
    }
    return state.StateMachineInstances;
});
export const selectStateMachineInstanceResult = createSelector(selectStateMachineInstance, (state) => {
    if (!state || !state.StateMachineInstance) {
        return null;
    }
    return state.StateMachineInstance;
});
export const selectEventMeshServersResult = createSelector(selectEventMeshServers, (state) => {
    if (!state || !state.EventMeshServers) {
        return null;
    }
    return state.EventMeshServers;
});
export const selectApplicationDomainsResult = createSelector(selectApplicationDomains, (state) => {
    if (!state || !state.ApplicationDomains) {
        return null;
    }
    return state.ApplicationDomains;
});
export const selectServerStatusResult = createSelector(selectServer, (state) => {
    if (!state || !state.Status) {
        return null;
    }
    return state.Status;
});
export const selectVpnLstResult = createSelector(selectVpns, (state) => {
    if (!state || !state.VpnLst) {
        return null;
    }
    return state.VpnLst;
});
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
    vpns: fromVpns.getVpnLstReducer
};
//# sourceMappingURL=appstate.js.map