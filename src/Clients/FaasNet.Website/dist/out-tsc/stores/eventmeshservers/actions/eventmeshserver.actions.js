import { createAction, props } from '@ngrx/store';
export const startAdd = createAction('[EventMeshServers] START_ADD_EVENTMESH_SERVER', props());
export const completeAdd = createAction('[EventMeshServers] COMPLETE_ADD_EVENTMESH_SERVER', props());
export const errorAdd = createAction('[EventMeshServers] ERROR_ADD_EVENTMESH_SERVER');
export const startGetAll = createAction('[EventMeshServers] START_GET_ALL_EVENTMESH_SERVERS');
export const completeGetAll = createAction('[EventMeshServers] COMPLETE_GET_ALL_EVENTMESH_SERVERS', props());
export const errorGetAll = createAction('[EventMeshServers] ERROR_GET_ALL_EVENTMESH_SERVERS');
export const startAddBridge = createAction('[EventMeshServers] START_ADD_BRIDGE', props());
export const completeAddBridge = createAction('[EventMeshServers] COMPLETE_ADD_BRIDGE');
export const errorAddBridge = createAction('[EventMeshServers] ERROR_ADD_BRIDGE');
//# sourceMappingURL=eventmeshserver.actions.js.map