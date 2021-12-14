import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/statemachineinstances.actions';
import { StateMachineInstanceDetails } from '../models/statemachineinstance-details.model';
import { StateMachineInstance } from '../models/statemachineinstance.model';

export interface SearchStateMachineInstanceState {
  StateMachineInstances: SearchResult<StateMachineInstance>;
}

export interface StateMachineInstanceState {
  StateMachineInstance: StateMachineInstanceDetails;
}

export const initialSearchStateMachineInstances: SearchStateMachineInstanceState = {
  StateMachineInstances: new SearchResult<StateMachineInstance>()
};

export const initialStateMachineInstanceState: StateMachineInstanceState = {
  StateMachineInstance: new StateMachineInstanceDetails()
};

const searchStateMachineInstancesReducer = createReducer(
  initialSearchStateMachineInstances,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      StateMachineInstances: { ...content }
    };
  })
);

const stateMachineInstanceReducer = createReducer(
  initialStateMachineInstanceState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      StateMachineInstance: { ...content }
    };
  })
);

export function getSearchStateMachineInstancesReducer(state: SearchStateMachineInstanceState | undefined, action: Action) {
  return searchStateMachineInstancesReducer(state, action);
}

export function getStateMachineInstanceReducer(state: StateMachineInstanceState | undefined, action: Action) {
  return stateMachineInstanceReducer(state, action);
}
