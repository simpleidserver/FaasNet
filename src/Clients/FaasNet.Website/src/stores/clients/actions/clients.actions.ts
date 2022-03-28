import { createAction, props } from '@ngrx/store';
import { ClientResult } from '../models/client.model';

export const startGetAllClients = createAction('[CLIENTS] START_GET_ALL_CLIENTS', props<{ vpn: string }>());
export const completeGetAllClients = createAction('[CLIENTS] COMPLETE_GET_ALL_CLIENTS', props<{ content: ClientResult[] }>());
export const errorGetAllClients = createAction('[CLIENTS] ERROR_GET_ALL_CLIENTS');
export const startGetClient = createAction('[CLIENTS] START_GET_CLIENT', props<{ id: string }>());
export const completeGetClient = createAction('[CLIENTS] COMPLETE_GET_CLIENT', props<{ content: ClientResult }>());
export const errorGetClient = createAction('[CLIENTS] ERROR_GET_CLIENT');
export const startDeleteClient = createAction('[CLIENTS] START_DELETE_CLIENT', props<{ id: string }>());
export const completeDeleteClient = createAction('[CLIENTS] COMPLETE_DELETE_CLIENT', props<{ id: string }>());
export const errorDeleteClient = createAction('[CLIENTS] ERROR_DELETE_CLIENT');
export const startAddClient = createAction('[CLIENTS] START_ADD_CLIENT', props<{ vpn: string, clientId: string, purposes: number[] }>());
export const completeAddClient = createAction('[CLIENTS] COMPLETE_ADD_CLIENT', props<{ id: string, vpn: string, clientId: string, purposes: number[] }>());
export const errorAddClient = createAction('[CLIENTS] ERROR_ADD_CLIENT');
