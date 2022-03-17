import { createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/eventmeshserver.actions';
export const initialEventMeshServersState = {
    EventMeshServers: []
};
const searchEventMeshServersReducer = createReducer(initialEventMeshServersState, on(fromActions.completeGetAll, (state, { content }) => {
    return Object.assign(Object.assign({}, state), { EventMeshServers: [...content] });
}));
export function getSearchEventMeshServersReducer(state, action) {
    return searchEventMeshServersReducer(state, action);
}
//# sourceMappingURL=index.js.map