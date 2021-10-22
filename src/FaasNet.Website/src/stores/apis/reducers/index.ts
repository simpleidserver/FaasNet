import { Action, createReducer, on } from '@ngrx/store';
import { _initialStateFactory } from '@ngrx/store/src/store_module';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/api.actions';
import { ApiDefinitionResult } from '../models/apidef.model';

export interface SearchApiDefsState {
  ApiDefs: SearchResult<ApiDefinitionResult>;
}

export interface ApiDefState {
  Details: ApiDefinitionResult | null;
}

export interface OperationState {
  InvocationResult: any | null;
}

export const initialSearchApiDefsState: SearchApiDefsState = {
  ApiDefs: new SearchResult<ApiDefinitionResult>()
};

export const initialApiDefState: ApiDefState = {
  Details: null
};

export const initialOperationState: OperationState = {
  InvocationResult: null
};

const searchApiDefsReducer = createReducer(
  initialSearchApiDefsState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      ApiDefs: { ...content }
    };
  })
);

const apiDefReducer = createReducer(
  initialApiDefState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      Details: { ...content }
    };
  })
);

const operationReducer = createReducer(
  initialOperationState,
  on(fromActions.completeInvokeOperation, (state, { content }) => {
    return {
      ...state,
      InvocationResult: { ...content}
    };
  })
);

export function getSearchApiDefsReducer(state: SearchApiDefsState | undefined, action: Action) {
  return searchApiDefsReducer(state, action);
}

export function getApiDefReducer(state: ApiDefState | undefined, action: Action) {
  return apiDefReducer(state, action);
}

export function getOperationReducer(state: OperationState | undefined, action: Action) {
  return operationReducer(state, action);
}
