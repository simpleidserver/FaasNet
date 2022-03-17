import { createAction, props } from '@ngrx/store';
export const startSearch = createAction('[ApiDefs] START_SEARCH_APIDEFS', props());
export const completeSearch = createAction('[ApiDefs] COMPLETE_SEARCH_APIDEFS', props());
export const errorSearch = createAction('[ApiDefs] ERROR_SEARCH_APIDEFS');
export const startGet = createAction('[ApiDefs] START_GET_APIDEF', props());
export const completeGet = createAction('[ApiDefs] COMPLETE_GET_APIDEF', props());
export const errorGet = createAction('[ApiDefs] ERROR_GET_APIDEF');
export const startAdd = createAction('[ApiDefs] START_ADD_APIDEF', props());
export const completeAdd = createAction('[ApiDefs] COMPLETE_ADD_APIDEF');
export const errorAdd = createAction('[ApiDefs] ERROR_ADD_APIDEF');
export const startAddOperation = createAction('[ApiDefs] START_ADD_APIDEF_OPERATION', props());
export const completeAddOperation = createAction('[ApiDefs] COMPLETE_ADD_APIDEF_OPERATION');
export const errorAddOperation = createAction('[ApiDefs] ERROR_ADD_APIDEF_OPERATION');
export const startUpdateUIOperation = createAction('[ApiDefs] START_UPDATE_UI_OPERATION', props());
export const completeUpdateUIOperation = createAction('[ApiDefs] COMPLETE_UPDATE_UI_OPERATION');
export const errorUpdateUIOperation = createAction('[ApiDefs] ERROR_UPDATE_UI_OPERATION');
export const startInvokeOperation = createAction('[ApiDefs] START_INVOKE_OPERATION', props());
export const completeInvokeOperation = createAction('[ApiDefs] COMPLETE_INVOKE_OPERATION', props());
export const errorInvokeOperation = createAction('[ApiDefs] ERROR_INVOKE_OPERATION');
//# sourceMappingURL=api.actions.js.map