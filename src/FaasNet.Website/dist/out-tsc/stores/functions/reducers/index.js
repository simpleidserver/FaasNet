import { createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/function.actions';
export const initialSearchFunctionsState = {
    Functions: new SearchResult()
};
export const initialFunctionState = {
    Configuration: null,
    Function: null,
    Threads: null,
    VirtualMemoryBytes: null,
    CpuUsage: null,
    RequestDuration: null,
    Details: null,
    TotalRequests: null
};
const searchFunctionsReducer = createReducer(initialSearchFunctionsState, on(fromActions.completeSearch, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Functions: Object.assign({}, content) });
}));
const functionReducer = createReducer(initialFunctionState, on(fromActions.completeGetConfiguration, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Configuration: Object.assign({}, content) });
}), on(fromActions.completeGet, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Function: Object.assign({}, content) });
}), on(fromActions.completeGetThreads, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Threads: content });
}), on(fromActions.completeGetVirtualMemoryBytes, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { VirtualMemoryBytes: content });
}), on(fromActions.completeGetCpuUsage, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { CpuUsage: content });
}), on(fromActions.completeGetRequestDuration, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { RequestDuration: content });
}), on(fromActions.completeGetDetails, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Details: content });
}), on(fromActions.completeGetTotalRequests, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { TotalRequests: content });
}));
export function getSearchFunctionsReducer(state, action) {
    return searchFunctionsReducer(state, action);
}
export function getFunctionReducer(state, action) {
    return functionReducer(state, action);
}
//# sourceMappingURL=index.js.map