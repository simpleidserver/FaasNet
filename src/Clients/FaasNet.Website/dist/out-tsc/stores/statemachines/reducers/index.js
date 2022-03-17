import { createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/statemachines.actions';
export const initialSearchStateMachines = {
    StateMachines: new SearchResult()
};
export const initialStateMachine = {
    StateMachine: null
};
const searchStateMachinesReducer = createReducer(initialSearchStateMachines, on(fromActions.completeSearch, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { StateMachines: Object.assign({}, content) });
}));
const stateMachineReducer = createReducer(initialStateMachine, on(fromActions.completeGetJson, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { StateMachine: Object.assign({}, content) });
}));
export function getSearchStateMachinesReducer(state, action) {
    return searchStateMachinesReducer(state, action);
}
export function getStateMachineReducer(state, action) {
    return stateMachineReducer(state, action);
}
//# sourceMappingURL=index.js.map