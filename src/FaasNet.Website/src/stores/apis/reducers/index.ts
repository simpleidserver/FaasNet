import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/api.actions';
import { ApiDefinitionResult } from '../models/apidef.model';

export interface SearchApiDefsState {
  ApiDefs: SearchResult<ApiDefinitionResult>;
}

export interface ApiDefState {
  Details: ApiDefinitionResult | null;
}

export const initialSearchApiDefsState: SearchApiDefsState = {
  ApiDefs: new SearchResult<ApiDefinitionResult>()
};

export const initialApiDefState: ApiDefState = {
  Details: null
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

export function getSearchApiDefsReducer(state: SearchApiDefsState | undefined, action: Action) {
  return searchApiDefsReducer(state, action);
}

export function getApiDefReducer(state: ApiDefState | undefined, action: Action) {
  return apiDefReducer(state, action);
}
