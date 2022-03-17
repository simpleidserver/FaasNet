import { createAction, props } from '@ngrx/store';
import { ServerStatusResult } from '../models/serverstatus.model';

export const startGetServerStatus = createAction('[SERVER] START_GET_STATUS');
export const completeGetServerStatus = createAction('[SERVER] COMPLETE_GET_STATUS', props<{ content : ServerStatusResult }>());
export const errorGetServerStatus = createAction('[SERVER] ERROR_GET_STATUS');
