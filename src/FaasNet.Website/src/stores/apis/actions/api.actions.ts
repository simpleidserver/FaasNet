import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { ApiDefinitionOperationUIResult, ApiDefinitionResult } from '../models/apidef.model';

export const startSearch = createAction('[ApiDefs] START_SEARCH_APIDEFS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[ApiDefs] COMPLETE_SEARCH_APIDEFS', props<{ content: SearchResult<ApiDefinitionResult> }>());
export const errorSearch = createAction('[ApiDefs] ERROR_SEARCH_APIDEFS');
export const startGet = createAction('[ApiDefs] START_GET_APIDEF', props<{ funcName: string }>());
export const completeGet = createAction('[ApiDefs] COMPLETE_GET_APIDEF', props<{ content: ApiDefinitionResult }>());
export const errorGet = createAction('[ApiDefs] ERROR_GET_APIDEF');
export const startAdd = createAction('[ApiDefs] START_ADD_APIDEF', props<{ name: string, path: string }>());
export const completeAdd = createAction('[ApiDefs] COMPLETE_ADD_APIDEF');
export const errorAdd = createAction('[ApiDefs] ERROR_ADD_APIDEF');
export const startAddOperation = createAction('[ApiDefs] START_ADD_APIDEF_OPERATION', props<{ funcName: string, opName: string, opPath: string }>())
export const completeAddOperation = createAction('[ApiDefs] COMPLETE_ADD_APIDEF_OPERATION');
export const errorAddOperation = createAction('[ApiDefs] ERROR_ADD_APIDEF_OPERATION');
export const startUpdateUIOperation = createAction('[ApiDefs] START_UPDATE_UI_OPERATION', props<{ funcName: string, operationName: string, ui: ApiDefinitionOperationUIResult }>());
export const completeUpdateUIOperation = createAction('[ApiDefs] COMPLETE_UPDATE_UI_OPERATION');
export const errorUpdateUIOperation = createAction('[ApiDefs] ERROR_UPDATE_UI_OPERATION');
