import { createSelector } from '@ngrx/store';
import * as fromApplications from './application/reducers';
import * as fromEventMeshServers from './eventmeshservers/reducers';
import * as fromFunctions from './functions/reducers';
import * as fromStateMachineInstances from './statemachineinstances/reducers';
import * as fromStateMachines from './statemachines/reducers';

export interface AppState {
  functions: fromFunctions.SearchFunctionsState;
  function: fromFunctions.FunctionState;
  stateMachines: fromStateMachines.SearchStateMachineState,
  stateMachine: fromStateMachines.StateMachineState,
  stateMachineInstances: fromStateMachineInstances.SearchStateMachineInstanceState,
  stateMachineInstance: fromStateMachineInstances.StateMachineInstanceState,
  eventMeshServers: fromEventMeshServers.EventMeshServersState,
  applicationDomains: fromApplications.ApplicationDomainsState
}

export const selectFunctions = (state: AppState) => state.functions;
export const selectFunction = (state: AppState) => state.function;
export const selectStateMachines = (state: AppState) => state.stateMachines;
export const selectStateMachine = (state: AppState) => state.stateMachine;
export const selectStateMachineInstances = (state: AppState) => state.stateMachineInstances;
export const selectStateMachineInstance = (state: AppState) => state.stateMachineInstance;
export const selectEventMeshServers = (state: AppState) => state.eventMeshServers;
export const selectApplicationDomains = (state: AppState) => state.applicationDomains;

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

export const appReducer = {
  functions: fromFunctions.getSearchFunctionsReducer,
  function: fromFunctions.getFunctionReducer,
  stateMachines: fromStateMachines.getSearchStateMachinesReducer,
  stateMachine: fromStateMachines.getStateMachineReducer,
  stateMachineInstances: fromStateMachineInstances.getSearchStateMachineInstancesReducer,
  stateMachineInstance: fromStateMachineInstances.getStateMachineInstanceReducer,
  eventMeshServers: fromEventMeshServers.getSearchEventMeshServersReducer,
  applicationDomains: fromApplications.getApplicationDomainsReducer
};
