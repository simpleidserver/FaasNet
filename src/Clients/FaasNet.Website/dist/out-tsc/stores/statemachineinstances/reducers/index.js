import { createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/statemachineinstances.actions';
import { StateMachineInstanceDetails } from '../models/statemachineinstance-details.model';
export const initialSearchStateMachineInstances = {
    StateMachineInstances: new SearchResult()
};
export const initialStateMachineInstanceState = {
    StateMachineInstance: new StateMachineInstanceDetails()
};
const searchStateMachineInstancesReducer = createReducer(initialSearchStateMachineInstances, on(fromActions.completeSearch, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { StateMachineInstances: Object.assign({}, content) });
}));
const stateMachineInstanceReducer = createReducer(initialStateMachineInstanceState, on(fromActions.completeGet, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { StateMachineInstance: Object.assign({}, content) });
}));
export function getSearchStateMachineInstancesReducer(state, action) {
    return searchStateMachineInstancesReducer(state, action);
}
export function getStateMachineInstanceReducer(state, action) {
    return stateMachineInstanceReducer(state, action);
}
//# sourceMappingURL=index.js.map