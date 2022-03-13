import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/eventmeshserver.actions';
import { EventMeshServerResult } from '../models/eventmeshserver.model';

export interface EventMeshServersState {
  EventMeshServers: EventMeshServerResult[];
}

export const initialEventMeshServersState: EventMeshServersState = {
  EventMeshServers: []
};

const searchEventMeshServersReducer = createReducer(
  initialEventMeshServersState,
  on(fromActions.completeGetAll, (state, { content }) => {
    return {
      ...state,
      EventMeshServers: [ ...content ]
    };
  })
);

export function getSearchEventMeshServersReducer(state: EventMeshServersState | undefined, action: Action) {
  return searchEventMeshServersReducer(state, action);
}
