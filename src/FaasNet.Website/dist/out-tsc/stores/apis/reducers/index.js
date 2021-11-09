import { createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/api.actions';
export const initialSearchApiDefsState = {
    ApiDefs: new SearchResult()
};
export const initialApiDefState = {
    Details: null
};
export const initialOperationState = {
    InvocationResult: null
};
const searchApiDefsReducer = createReducer(initialSearchApiDefsState, on(fromActions.completeSearch, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { ApiDefs: Object.assign({}, content) });
}));
const apiDefReducer = createReducer(initialApiDefState, on(fromActions.completeGet, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Details: Object.assign({}, content) });
}));
const operationReducer = createReducer(initialOperationState, on(fromActions.completeInvokeOperation, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { InvocationResult: Object.assign({}, content) });
}));
export function getSearchApiDefsReducer(state, action) {
    return searchApiDefsReducer(state, action);
}
export function getApiDefReducer(state, action) {
    return apiDefReducer(state, action);
}
export function getOperationReducer(state, action) {
    return operationReducer(state, action);
}
//# sourceMappingURL=index.js.map