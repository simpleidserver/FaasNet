import { createAction, props } from '@ngrx/store';
export const startGetServerStatus = createAction('[SERVER] START_GET_STATUS');
export const completeGetServerStatus = createAction('[SERVER] COMPLETE_GET_STATUS', props());
export const errorGetServerStatus = createAction('[SERVER] ERROR_GET_STATUS');
//# sourceMappingURL=server.actions.js.map