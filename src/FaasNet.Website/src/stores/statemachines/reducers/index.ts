import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/statemachines.actions';
import { StateMachine } from '../models/statemachine.model';
import { StateMachineModel } from '../models/statemachinemodel.model';

export interface SearchStateMachineState {
  StateMachines: SearchResult<StateMachine>;
}

export interface StateMachineState {
  StateMachine: StateMachineModel;
}

export const initialSearchStateMachines: SearchStateMachineState = {
  StateMachines: new SearchResult<StateMachine>()
};

export const initialStateMachine: StateMachineState = {
  StateMachine: new StateMachineModel()
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
    const stateMachine = StateMachineModel.build(content);
    return {
      ...state,
      StateMachine: stateMachine
    };
  })
);

export function getSearchStateMachinesReducer(state: SearchStateMachineState | undefined, action: Action) {
  return searchStateMachinesReducer(state, action);
}

export function getStateMachineReducer(state: StateMachineState | undefined, action: Action) {
  return stateMachineReducer(state, action);
}
