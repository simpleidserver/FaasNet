import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../common/search.model';
import * as fromActions from '../actions/function.actions';
import { FunctionResult } from '../models/function.model';

export interface SearchFunctionsState {
  Functions: SearchResult<FunctionResult>;
}

export interface FunctionState {
  Configuration: any | null;
  Function: FunctionResult | null;
}

export const initialSearchFunctionsState: SearchFunctionsState = {
  Functions: new SearchResult<FunctionResult>()
};

export const initialFunctionState: FunctionState = {
  Configuration: null,
  Function : null
};

const searchFunctionsReducer = createReducer(
  initialSearchFunctionsState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      Functions: { ...content }
    };
  })
);

const functionReducer = createReducer(
  initialFunctionState,
  on(fromActions.completeGetConfiguration, (state, { content }) => {
    return {
      ...state,
      Configuration: { ...content }
    };
  }),
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      Function: { ...content }
    };
  }),
);

export function getSearchFunctionsReducer(state: SearchFunctionsState | undefined, action: Action) {
  return searchFunctionsReducer(state, action);
}

export function getFunctionReducer(state: FunctionState | undefined, action: Action) {
  return functionReducer(state, action);
}
