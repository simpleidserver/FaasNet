import { createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/server.actions';
import { ServerStatusResult } from '../models/serverstatus.model';
export const initialServerState = {
    Status: new ServerStatusResult()
};
const serverReducer = createReducer(initialServerState, on(fromActions.completeGetServerStatus, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { Status: Object.assign({}, content) });
}));
export function getServerReducer(state, action) {
    return serverReducer(state, action);
}
//# sourceMappingURL=index.js.map