import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/statemachineinstances.actions';
import { StateMachineInstance } from '../models/statemachineinstance.model';

export interface SearchStateMachineInstanceState {
  StateMachineInstances: SearchResult<StateMachineInstance>;
}

export const initialSearchStateMachineInstances: SearchStateMachineInstanceState = {
  StateMachineInstances: new SearchResult<StateMachineInstance>()
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

export function getSearchStateMachineInstancesReducer(state: SearchStateMachineInstanceState | undefined, action: Action) {
  return searchStateMachineInstancesReducer(state, action);
}
