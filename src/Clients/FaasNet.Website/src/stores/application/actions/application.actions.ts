import { createAction, props } from '@ngrx/store';
import { ApplicationDomainResult } from '../models/applicationdomain.model';

export const startAddApplicationDomain = createAction('[Applications] START_ADD_APPLICATION_DOMAIN', props<{ rootTopic: string, name: string, description: string }>());
export const completeAddApplicationDomain = createAction('[Applications] COMPLETE_ADD_APPLICATION_DOMAIN', props<{ content: ApplicationDomainResult }>());
export const errorAddApplicationDomain = createAction('[Applications] ERROR_ADD_APPLICATION_DOMAIN');
export const startGetAllApplicationDomains = createAction('[Applications] START_GET_ALL_APPLICATION_DOMAINS');
export const completeGetAllApplicationDomains = createAction('[Applications] COMPLETE_GET_ALL_APPLICATION_DOMAINS', props<{ content: ApplicationDomainResult[] }>());
export const errorGetAllApplicationDomains = createAction('[Applications] ERROR_GET_ALL_APPLICATION_DOMAINS');
