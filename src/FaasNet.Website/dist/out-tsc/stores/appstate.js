import { createSelector } from '@ngrx/store';
import * as fromApiDefs from './apis/reducers';
import * as fromFunctions from './functions/reducers';
export const selectFunctions = (state) => state.functions;
export const selectFunction = (state) => state.function;
export const selectApiDefs = (state) => state.apiDefs;
export const selectApiDef = (state) => state.apiDef;
export const selectOperation = (state) => state.operation;
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
export const selectApiDefResult = createSelector(selectApiDef, (state) => {
    if (!state || state.Details === null) {
        return null;
    }
    return state.Details;
});
export const selectApiDefsResult = createSelector(selectApiDefs, (state) => {
    if (!state || !state.ApiDefs) {
        return null;
    }
    return state.ApiDefs;
});
export const selectApiOperationInvocationResult = createSelector(selectOperation, (state) => {
    if (!state || !state.InvocationResult) {
        return null;
    }
    return state.InvocationResult;
});
export const appReducer = {
    functions: fromFunctions.getSearchFunctionsReducer,
    function: fromFunctions.getFunctionReducer,
    apiDef: fromApiDefs.getApiDefReducer,
    apiDefs: fromApiDefs.getSearchApiDefsReducer,
    operation: fromApiDefs.getOperationReducer
};
//# sourceMappingURL=appstate.js.map