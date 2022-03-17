import { createAction, props } from '@ngrx/store';
export const startAddApplicationDomain = createAction('[Applications] START_ADD_APPLICATION_DOMAIN', props());
export const completeAddApplicationDomain = createAction('[Applications] COMPLETE_ADD_APPLICATION_DOMAIN', props());
export const errorAddApplicationDomain = createAction('[Applications] ERROR_ADD_APPLICATION_DOMAIN');
export const startGetAllApplicationDomains = createAction('[Applications] START_GET_ALL_APPLICATION_DOMAINS');
export const completeGetAllApplicationDomains = createAction('[Applications] COMPLETE_GET_ALL_APPLICATION_DOMAINS', props());
export const errorGetAllApplicationDomains = createAction('[Applications] ERROR_GET_ALL_APPLICATION_DOMAINS');
//# sourceMappingURL=application.actions.js.map