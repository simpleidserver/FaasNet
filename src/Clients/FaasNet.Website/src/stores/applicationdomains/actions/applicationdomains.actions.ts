import { createAction, props } from '@ngrx/store';
import { AppDomainResult } from '../models/appdomain.model';
import { ApplicationResult } from '../models/application.model';

export const startGetAppDomain = createAction('[APPLICATIONDOMAINS] START_GET_APPDOMAIN', props<{ id: string }>());
export const completeGetAppDomain = createAction('[APPLICATIONDOMAINS] COMPLETE_GET_APPDOMAIN', props<{ content: AppDomainResult }>());
export const errorGetAppDomain = createAction('[APPLICATIONDOMAINS] ERROR_GET_APPDOMAIN');
export const startGetAllAppDomains = createAction('[APPLICATIONDOMAINS] START_GET_ALL_APPDOMAINS', props<{ vpn: string }>());
export const completeGetAllAppDomains = createAction('[APPLICATIONDOMAINS] COMPLETE_GET_ALL_APPDOMAINS', props<{ content: AppDomainResult[] }>());
export const errorGetAllAppDomains = createAction('[APPLICATIONDOMAINS] ERROR_GET_ALL_APPDOMAINS');
export const startAddAppDomain = createAction('[APPLICATIONDOMAINS] START_ADD_APPDOMAIN', props<{ vpn: string, name: string, description: string, rootTopic: string }>());
export const completeAddAppDomain = createAction('[APPLICATIONDOMAINS] COMPLETE_ADD_APPDOMAIN', props<{ id: string, name: string, description: string, rootTopic: string }>());
export const errorAddAppDomain = createAction('[APPLICATIONDOMAINS] ERROR_ADD_APPDOMAIN');
export const startDeleteAppDomain = createAction('[APPLICATIONDOMAINS] START_DELETE_APPDOMAIN', props<{ id: string }>());
export const completeDeleteAppDomain = createAction('[APPLICATIONDOMAINS] COMPLETE_DELETE_APPDOMAIN', props<{ id: string }>());
export const errorDeleteAppDomain = createAction('[APPLICATIONDOMAINS] ERROR_DELETE_APPDOMAIN');
export const updateApplicationDomain = createAction('[APPLICATIONDOMAINS] UPDATE_APPLICATION_DOMAIN', props<{ id: string, applications: ApplicationResult[] }>());
export const completeUpdateApplicationDomain = createAction('[APPLICATIONDOMAINS] COMPLETE_UPDATE_APPLICATION_DOMAIN', props<{ id: string, applications: ApplicationResult[] }>());
export const errorUpdateApplicationDomain = createAction('[APPLICATIONDOMAINS] ERROR_UPDATE_APPLICATION_DOMAIN');
