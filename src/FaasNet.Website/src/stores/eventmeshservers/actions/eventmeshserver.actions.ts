import { createAction, props } from '@ngrx/store';
import { EventMeshServerResult } from '../models/eventmeshserver.model';

export const startAdd = createAction('[EventMeshServers] START_ADD_EVENTMESH_SERVER', props<{ isLocalhost: boolean, urn: string, port: number }>());
export const completeAdd = createAction('[EventMeshServers] COMPLETE_ADD_EVENTMESH_SERVER', props<{ content: EventMeshServerResult }>());
export const errorAdd = createAction('[EventMeshServers] ERROR_ADD_EVENTMESH_SERVER');
export const startGetAll = createAction('[EventMeshServers] START_GET_ALL_EVENTMESH_SERVERS');
export const completeGetAll = createAction('[EventMeshServers] COMPLETE_GET_ALL_EVENTMESH_SERVERS', props<{ content: EventMeshServerResult[] }>());
export const errorGetAll = createAction('[EventMeshServers] ERROR_GET_ALL_EVENTMESH_SERVERS');
