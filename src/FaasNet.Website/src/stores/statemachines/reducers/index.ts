import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/statemachines.actions';
import { StateMachine } from '../models/statemachine.model';
import { StateMachineModel } from '../models/statemachinemodel.model';

export interface SearchStateMachineState {
  StateMachines: SearchResult<StateMachine>;
}

export interface StateMachineState {
  StateMachine: any;
}

export const initialSearchStateMachines: SearchStateMachineState = {
  StateMachines: new SearchResult<StateMachine>()
};

export const initialStateMachine: StateMachineState = {
  StateMachine: null
};

const searchStateMachinesReducer = createReducer(
  initialSearchStateMachines,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      StateMachines: { ...content }
    };
  })
);

const stateMachineReducer = createReducer(
  initialStateMachine,
  on(fromActions.completeGetJson, (state, { content }) => {
    return {
      ...state,
      StateMachine: { ...content }
    };
  })
);

export function getSearchStateMachinesReducer(state: SearchStateMachineState | undefined, action: Action) {
  return searchStateMachinesReducer(state, action);
}

export function getStateMachineReducer(state: StateMachineState | undefined, action: Action) {
  return stateMachineReducer(state, action);
}
