import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import { FunctionResult } from '../models/function.model';

export const startSearch = createAction('[Functions] START_SEARCH_FUNCTIONS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Functions] COMPLETE_SEARCH_FUNCTIONS', props<{ content: SearchResult<FunctionResult> }>());
export const errorSearch = createAction('[Functions] ERROR_SEARCH_FUNCTIONS');
export const startGetConfiguration = createAction('[Functions] START_GET_FUNCTION_CONFIGURATION', props<{ name: string }>());
export const completeGetConfiguration = createAction('[Functions] COMPLETE_GET_FUNCTION_CONFIGURATION', props<{ content: any }>());
export const errorGetConfiguration = createAction('[Functions] ERROR_GET_FUNCTION_CONFIGURATION');
export const startInvoke = createAction('[Functions] START_INVOKE_FUNCTION', props<{ name: string, request: any }>());
export const completeInvoke = createAction('[Functions] COMPLETE_INVOKE_FUNCTION', props<{ content: any }>());
export const errorInvoke = createAction('[Functions] ERROR_INVOKE_FUNCTION');
export const startGet = createAction('[Functions] START_GET_FUNCTION', props<{ name: string }>());
export const completeGet = createAction('[Functions] COMPLETE_GET_FUNCTION', props<{ content: FunctionResult}>());
export const errorGet = createAction('[Functions] ERROR_GET_FUNCTION');
export const startDelete = createAction('[Functions] START_DELETE_FUNCTION', props<{ name: string }>());
export const completeDelete = createAction('[Functions] COMPLETE_DELETE_FUNCTION');
export const errorDelete = createAction('[Functions] ERROR_DELETE_FUNCTION');
