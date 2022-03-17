import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/server.actions';
import { ServerStatusResult } from '../models/serverstatus.model';

export interface ServerState {
  Status: ServerStatusResult;
}

export const initialServerState: ServerState = {
  Status: new ServerStatusResult()
};

const serverReducer = createReducer(
  initialServerState,
  on(fromActions.completeGetServerStatus, (state, { content }) => {
    return {
      ...state,
      Status: { ...content }
    };
  })
);

export function getServerReducer(state: ServerState | undefined, action: Action) {
  return serverReducer(state, action);
}
